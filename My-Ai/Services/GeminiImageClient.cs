using Mscc.GenerativeAI;
using My_Ai.Services;

namespace My_Ai.Services
{
    public class GeminiImageClient : GeminiClient
    {
        public GeminiImageClient(IConfiguration configuration) : base(configuration, Model.Gemini25FlashImagePreview)
        {

        }

        protected override async Task<(string tempPath, string mimeType)> PrepareFileAsync(IFormFile file)
        {
            var mime = file.ContentType;
            var allowed =
                mime == "application/pdf" ||
                mime == "text/plain" ||
                mime.StartsWith("image/", StringComparison.OrdinalIgnoreCase);

            if (!allowed)
            {
                throw new InvalidOperationException($"Unsupported MIME type: {mime}");
            }
            var tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + Path.GetExtension(file.FileName));
            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return (tempPath, mime);
        }
    }

}
