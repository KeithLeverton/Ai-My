using Mscc.GenerativeAI;

namespace My_Ai.Services
{
    public interface IClient
    {
        Task<string> GenerateResponse(string prompt);
        Task<string> GenerateResponse(string prompt, IFormFile file);

    }
}
