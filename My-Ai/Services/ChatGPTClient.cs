#pragma warning disable OPENAI001
using OpenAI.Chat;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
#pragma warning restore OPENAI001

namespace My_Ai.Services
{
    /// <summary>
    /// ChatGPT client supporting:
    ///  - Direct chat (text-only)
    ///  - Multimodal inline (text + image)
    ///  - File processing (DOCX, text files)
    /// </summary>
    public class ChatGPTClient : IClient
    {
        private readonly ChatClient _chatClient;
        private readonly IConfiguration _configuration;
        private readonly string _model;

        public ChatGPTClient(IConfiguration configuration)
        {
            _configuration = configuration;
            _model = configuration["OpenAI:ChatModel"] ?? "gpt-4.1";
            var apiKey = configuration["ChatGPTApiKey"] ?? throw new InvalidOperationException("ChatGPTApiKey not configured.");

            _chatClient = new ChatClient(model: _model, apiKey: apiKey);
        }

        // ========== IClient implementation for Gemini compatibility ==========
        public async Task<string> GenerateResponse(string prompt, IFormFile file)
        {
            var result = await GenerateResponseInternal(prompt, file);
            // Create a mock GenerateContentResponse for interface compatibility
            return result;
        }

        // ========== Simple chat ==========
        public async Task<string> GenerateResponse(string prompt)
        {
            var response = await _chatClient.CompleteChatAsync(prompt);
            return response.Value.Content.FirstOrDefault()?.Text ?? string.Empty;
        }

        // ========== Internal implementation ==========
        private async Task<string> GenerateResponseInternal(string prompt, IFormFile file)
        {
            CancellationToken cancellationToken = default;
            ValidateFile(file);

            // Size limits for different approaches
            const int maxInlineFileSize = 20 * 1024 * 1024; // 20MB for vision
            const int maxTextSize = 100_000; // ~100k chars for text

            if (IsImage(file))
            {
                if (file.Length > maxInlineFileSize)
                    throw new ArgumentException($"Image too large. Max size: {maxInlineFileSize / (1024 * 1024)}MB");

                return await ProcessImageInline(prompt, file, cancellationToken);
            }
            else
            {
                string fileText = await ExtractFileText(file, cancellationToken);

                // For very large documents, truncate with warning
                if (fileText.Length > maxTextSize)
                {
                    fileText = fileText[..maxTextSize] + "\n\n[Note: File was truncated due to size. Consider splitting large documents.]";
                }

                return await ProcessTextInline(prompt, fileText, file.FileName, cancellationToken);
            }
        }

        // ========== Private Methods ==========
        private async Task<string> ProcessImageInline(string prompt, IFormFile file, CancellationToken ct)
        {
            try
            {
                // More aggressive resizing to stay under data URI limits
                var processedBytes = await ResizeImageForDataUri(file, ct);
                var base64 = Convert.ToBase64String(processedBytes.bytes);
                var dataUri = $"data:{processedBytes.mimeType};base64,{base64}";

                var userMessage = new UserChatMessage(new List<ChatMessageContentPart>
        {
            ChatMessageContentPart.CreateTextPart(prompt),
            ChatMessageContentPart.CreateImagePart(new Uri(dataUri))
        });

                var response = await WithRetry(() => _chatClient.CompleteChatAsync([userMessage], cancellationToken: ct));
                return response.Value.Content.FirstOrDefault(c => c.Kind == ChatMessageContentPartKind.Text)?.Text ?? string.Empty;
            }
            catch (UriFormatException)
            {
                throw new ArgumentException("Image is too large for processing. Please use a smaller image file.");
            }
        }

        private async Task<(byte[] bytes, string mimeType)> ResizeImageForDataUri(IFormFile file, CancellationToken ct)
        {
            var originalBytes = await ToBytesAsync(file, ct);

            // Target much smaller size for data URIs - aim for ~300-500KB
            const int targetSizeBytes = 400 * 1024; // 400KB target
            const int maxDataUriBytes = 1024 * 1024; // 1MB absolute max for data URI

            // If already small enough, return as-is
            if (originalBytes.Length <= targetSizeBytes)
            {
                return (originalBytes, file.ContentType);
            }

            try
            {
                using var originalStream = new MemoryStream(originalBytes);
                using var image = Image.Load(originalStream);

                // More aggressive dimension limits for data URIs
                var maxDimension = 512; // Smaller max dimension
                var ratio = Math.Min((double)maxDimension / image.Width, (double)maxDimension / image.Height);

                var newWidth = (int)(image.Width * ratio);
                var newHeight = (int)(image.Height * ratio);

                image.Mutate(x => x.Resize(newWidth, newHeight));

                // Try different quality levels to hit target size
                var qualities = new[] { 60, 50, 40, 30 };

                foreach (var quality in qualities)
                {
                    using var outputStream = new MemoryStream();
                    var encoder = new JpegEncoder() { Quality = quality };

                    await image.SaveAsJpegAsync(outputStream, encoder, ct);
                    var resultBytes = outputStream.ToArray();

                    // If we hit our target or this is the last attempt
                    if (resultBytes.Length <= targetSizeBytes || quality == qualities.Last())
                    {
                        if (resultBytes.Length > maxDataUriBytes)
                        {
                            throw new ArgumentException($"Image too large even after compression. Size: {resultBytes.Length / 1024}KB, Max: {maxDataUriBytes / 1024}KB");
                        }

                        return (resultBytes, "image/jpeg");
                    }
                }

                throw new ArgumentException("Unable to compress image to acceptable size.");
            }
            catch (Exception ex) when (!(ex is ArgumentException))
            {
                throw new ArgumentException($"Unable to process image: {ex.Message}. Please use a smaller image file.");
            }
        }

        private async Task<string> ProcessTextInline(string prompt, string fileText, string fileName, CancellationToken ct)
        {
            var combinedPrompt = $@"{prompt}

    --- Begin File: {fileName} ---
    {fileText}
    --- End File ---";

            var response = await WithRetry(() => _chatClient.CompleteChatAsync(combinedPrompt));
            return response.Value.Content.FirstOrDefault()?.Text ?? string.Empty;
        }

        private async Task<string> ExtractFileText(IFormFile file, CancellationToken ct)
        {
            if (IsDocx(file))
            {
                return await ExtractDocxTextAsync(file, ct);
            }
            else if (IsPlainTextLike(file))
            {
                using var reader = new StreamReader(file.OpenReadStream());
                return await reader.ReadToEndAsync(ct);
            }
            else if (IsPdf(file))
            {
                throw new NotSupportedException("PDF text extraction not implemented. Please convert to text or DOCX format.");
            }
            else
            {
                throw new NotSupportedException($"File type '{file.ContentType}' not supported. Supported: images, text files, DOCX.");
            }
        }

        private void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or not provided.");

            const long maxSize = 25 * 1024 * 1024; // 25MB
            if (file.Length > maxSize)
                throw new ArgumentException($"File too large. Max size: {maxSize / (1024 * 1024)}MB");

            var allowedTypes = new[] { "image/", "text/", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" };
            if (!allowedTypes.Any(type => file.ContentType.StartsWith(type)))
                throw new ArgumentException($"File type '{file.ContentType}' not supported. Supported: images, text files, DOCX.");
        }

        private async Task<T> WithRetry<T>(Func<Task<T>> operation, int maxRetries = 3)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    return await operation();
                }
                catch (HttpRequestException) when (i < maxRetries - 1)
                {
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i))); // Exponential backoff
                }
            }
            throw new InvalidOperationException("Max retries exceeded");
        }

        // ===== File Type Helpers =====
        private static bool IsImage(IFormFile file) =>
            file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);

        private static bool IsDocx(IFormFile file) =>
            file.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document" ||
            Path.GetExtension(file.FileName).Equals(".docx", StringComparison.OrdinalIgnoreCase);

        private static bool IsPlainTextLike(IFormFile file)
        {
            if (file.ContentType.StartsWith("text/", StringComparison.OrdinalIgnoreCase))
                return true;
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            return ext is ".txt" or ".md" or ".csv" or ".json" or ".log";
        }

        private static bool IsPdf(IFormFile file) =>
            file.ContentType == "application/pdf" ||
            Path.GetExtension(file.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase);

        private static async Task<byte[]> ToBytesAsync(IFormFile file, CancellationToken ct)
        {
            await using var s = file.OpenReadStream();
            using var ms = new MemoryStream();
            await s.CopyToAsync(ms, ct);
            return ms.ToArray();
        }

        private static async Task<string> ExtractDocxTextAsync(IFormFile file, CancellationToken ct)
        {
            await using var src = file.OpenReadStream();
            using var ms = new MemoryStream();
            await src.CopyToAsync(ms, ct);
            ms.Position = 0;

            using var doc = WordprocessingDocument.Open(ms, false);
            var body = doc.MainDocumentPart?.Document?.Body;
            if (body is null)
                return string.Empty;

            var sb = new System.Text.StringBuilder();
            foreach (var para in body.Elements())
            {
                foreach (var text in para.Descendants<Text>())
                    sb.Append(text.Text);
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
