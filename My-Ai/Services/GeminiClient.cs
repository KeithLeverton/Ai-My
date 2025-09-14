using Mscc.GenerativeAI;

namespace My_Ai.Services
{
    public abstract class GeminiClient
    {
        private readonly GoogleAI _googleAI;
        protected readonly GenerativeModel _model;
        private readonly IConfiguration _configuration;

        public GeminiClient(IConfiguration configuration, string model)
        {
            _configuration = configuration;
            _googleAI = new GoogleAI(configuration["GeminiApiKey"]);
            _model = _googleAI.GenerativeModel(model: model);
        }

        public async Task<GenerateContentResponse> GenerateResponse(string prompt, IFormFile fileContent)
        {
            var (tempPath, mimeType) = await PrepareFileAsync(fileContent);

            try
            {
                var request = new GenerateContentRequest(prompt);
                await request.AddMedia(tempPath, mimeType);
                var requestOptions = new RequestOptions()
                {
                    Timeout = TimeSpan.FromSeconds(int.Parse(_configuration["RequestTimeoutSeconds"] ?? "10000"))
                };
                var response = await _model.GenerateContent(request, requestOptions);
                return response;
            }
            finally
            {
                try { System.IO.File.Delete(tempPath); } catch { /* ignore cleanup errors */ }
            }
        }

        protected abstract Task<(string tempPath, string mimeType)> PrepareFileAsync(IFormFile file);
    }
}
