using Mscc.GenerativeAI;

namespace My_Ai.Services
{
    public interface IGeminiClient
    {
        Task<GenerateContentResponse> GenerateResponse(string prompt);
        Task<GenerateContentResponse> GenerateResponse(string prompt, IFormFile file);

    }
}
