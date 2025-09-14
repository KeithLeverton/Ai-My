using OpenAI.Chat;
using OpenAI.Images;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;

namespace My_Ai.Services
{
    public class OpenAIImageGenerationClient : IImageClient
    {
        private readonly ImageClient _imageClient;
        private readonly ImageClient _dalle2Client;
        private readonly ChatClient _visionClient;
        private readonly string _apiKey;
        
        public OpenAIImageGenerationClient(IConfiguration configuration)
        {
            _apiKey = configuration["ChatGPTApiKey"] ?? throw new InvalidOperationException("ChatGPTApiKey not configured.");
            _imageClient = new ImageClient("dall-e-3", _apiKey);
            _dalle2Client = new ImageClient("gpt-image-1", _apiKey);
            _visionClient = new ChatClient("gpt-4o", _apiKey);
        }

        // Implement IImageClient interface
        public async Task<string> GenerateResponse(IFormFile inputImage, string prompt)
        {
            return await GenerateImageFromInput(inputImage, prompt);
        }

        /// <summary>
        /// Generates an image based on an input image and a custom prompt using DALL-E 2.
        /// Converts to PNG format and ensures under 4MB as required by DALL-E 2.
        /// </summary>
        public async Task<string> GenerateImageFromInput(IFormFile inputImage, string prompt)
        {
            try
            {
                // Process image to meet DALL-E 2 requirements (PNG, under 4MB)
                var processedImageStream = await PrepareImageForDallE2(inputImage);
                
                // Use DALL-E 2 for image editing with direct stream input
                var imageResult = await _dalle2Client.GenerateImageEditAsync(
                    processedImageStream,
                    "image.png", // Always use .png extension
                    prompt,
                    new ImageEditOptions
                    {
                        Size = GeneratedImageSize.W1024xH1024,
// ResponseFormat = GeneratedImageFormat.Uri
                    });
                
                // Clean up the temporary stream
                await processedImageStream.DisposeAsync();

                if (imageResult.Value.ImageBytes != null)
                {
                    var imageBytes = imageResult.Value.ImageBytes.ToArray();
                    var base64String = Convert.ToBase64String(imageBytes);
                    return $"data:image/png;base64,{base64String}";
                }

                throw new InvalidOperationException("No image URI received from DALL-E 2");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to generate image with DALL-E 2: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Prepares image for DALL-E 2: converts to PNG format and ensures under 4MB
        /// </summary>
        private async Task<Stream> PrepareImageForDallE2(IFormFile inputImage)
        {
            // Read the original image
            var originalBytes = await ToBytesAsync(inputImage);
            
            using var originalStream = new MemoryStream(originalBytes);
            using var image = Image.Load(originalStream);
            
            // Start with original dimensions
            var width = image.Width;
            var height = image.Height;
            
            // Target 4MB limit with some buffer (3.5MB target)
            const int maxSizeBytes = 3500000; // 3.5MB
            var outputStream = new MemoryStream();
            
            // Try different sizes until we get under 4MB
            var scaleFactor = 1.0;
            byte[] resultBytes;
            
            do
            {
                outputStream.SetLength(0);
                outputStream.Position = 0;
                
                // Calculate new dimensions
                var newWidth = (int)(width * scaleFactor);
                var newHeight = (int)(height * scaleFactor);
                // using var resizedImage = image.Clone();
                // With the following:
                using var resizedImage = image.CloneAs<SixLabors.ImageSharp.PixelFormats.Rgba32>();
                resizedImage.Mutate(x => x.Resize(newWidth, newHeight));
                
                // Save as PNG
                await resizedImage.SaveAsPngAsync(outputStream);
                resultBytes = outputStream.ToArray();
                
                // If still too large, reduce scale factor
                if (resultBytes.Length > maxSizeBytes)
                {
                    scaleFactor *= 0.8; // Reduce by 20% each iteration
                }
                
                // Safety check to prevent infinite loop
                if (scaleFactor < 0.1)
                {
                    throw new InvalidOperationException("Cannot compress image to under 4MB while maintaining reasonable quality.");
                }
                
            } while (resultBytes.Length > maxSizeBytes);
            
            // Return a new stream with the processed PNG data
            return new MemoryStream(resultBytes);
        }

        /// <summary>
        /// Alternative method using DALL-E 2 image variations instead of editing
        /// </summary>
        public async Task<string> GenerateImageVariation(IFormFile inputImage)
        {
            try
            {
                var processedImageStream = await PrepareImageForDallE2(inputImage);
                
                var imageResult = await _dalle2Client.GenerateImageVariationAsync(
                    processedImageStream,
                    "image.png",
                    new ImageVariationOptions
                    {
                        Size = GeneratedImageSize.W1024xH1024,
                        ResponseFormat = GeneratedImageFormat.Uri
                    });
                
                await processedImageStream.DisposeAsync();
                
                if (imageResult.Value.ImageUri != null)
                {
                    return imageResult.Value.ImageUri.ToString();
                }
                
                throw new InvalidOperationException("No image URI received from DALL-E 2");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to generate image variation with DALL-E 2: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Generates an image from text prompt only using DALL-E 3 (highest quality)
        /// </summary>
        public async Task<string> GenerateImageFromText(string prompt)
        {
            try
            {
                var imageResult = await _imageClient.GenerateImageAsync(prompt, new ImageGenerationOptions
                {
                    Size = GeneratedImageSize.W1024xH1024,
                    Quality = GeneratedImageQuality.Standard,
                    ResponseFormat = GeneratedImageFormat.Uri
                });
                
                if (imageResult.Value.ImageUri != null)
                {
                    return imageResult.Value.ImageUri.ToString();
                }
                
                throw new InvalidOperationException("No image URI received from DALL-E 3");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to generate image from text: {ex.Message}", ex);
            }
        }

        private async Task<byte[]> ToBytesAsync(IFormFile file)
        {
            await using var s = file.OpenReadStream();
            using var ms = new MemoryStream();
            await s.CopyToAsync(ms);
            return ms.ToArray();
        }
    }
}