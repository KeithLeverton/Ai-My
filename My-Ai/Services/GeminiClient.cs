using Mscc.GenerativeAI;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace My_Ai.Services
{
    public class GeminiClient : IGeminiClient
    {
        private readonly GoogleAI _googleAI;
        private readonly GenerativeModel _model;
        private readonly IConfiguration _configuration;

        public GeminiClient(IConfiguration configuration)
        {
            _configuration = configuration;
            _googleAI = new GoogleAI(configuration["GeminiApiKey"]);
            _model = _googleAI.GenerativeModel(model: Model.Gemini20FlashLite);
        }

        public async Task<GenerateContentResponse> GenerateResponse(string prompt)
        {
            var request = new GenerateContentRequest(prompt);
            var response = await _model.GenerateContent(request);
            return response;
        }

        public async Task<GenerateContentResponse> GenerateResponse(string prompt, IFormFile fileContent)
        {
            // Prepare a file on disk that uses a MIME type accepted by the API
            var (tempPath, mimeType) = await PrepareFileAsync(fileContent);

            try
            {
                var request = new GenerateContentRequest(prompt);
                await request.AddMedia(tempPath, mimeType);
                var response = await _model.GenerateContent(request);
                return response;
            }
            finally
            {
                try { System.IO.File.Delete(tempPath); } catch { /* ignore cleanup errors */ }
            }
        }

        private static async Task<(string tempPath, string mimeType)> PrepareFileAsync(IFormFile file)
        {
            // Handle DOCX by extracting text and sending as text/plain
            if (file.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document" ||
                Path.GetExtension(file.FileName).Equals(".docx", StringComparison.OrdinalIgnoreCase))
            {
                var text = await ExtractDocxTextAsync(file);
                var txtPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".txt");
                await File.WriteAllTextAsync(txtPath, text);
                return (txtPath, "text/plain");
            }

            // Allowed pass-through MIME types
            var mime = file.ContentType;
            var allowed =
                mime == "application/pdf" ||
                mime == "text/plain" ||
                mime.StartsWith("image/", StringComparison.OrdinalIgnoreCase) ||
                mime.StartsWith("audio/", StringComparison.OrdinalIgnoreCase) ||
                mime.StartsWith("video/", StringComparison.OrdinalIgnoreCase);

            if (!allowed)
            {
                throw new NotSupportedException($"File type '{mime}' is not supported. Please upload PDF, TXT, or DOCX.");
            }

            // Copy to a temp file
            var tmp = Path.GetTempFileName();
            await using (var src = file.OpenReadStream())
            await using (var dst = System.IO.File.Create(tmp))
            {
                await src.CopyToAsync(dst);
            }
            return (tmp, mime);
        }

        private static async Task<string> ExtractDocxTextAsync(IFormFile file)
        {
            await using var src = file.OpenReadStream();
            using var ms = new MemoryStream();
            await src.CopyToAsync(ms);
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
