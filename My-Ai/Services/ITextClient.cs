using Mscc.GenerativeAI;

namespace My_Ai.Services
{
    public interface ITextClient : IClient
    {
        public Task<GenerateContentResponse> GenerateResponse(string prompt);

    }
}
