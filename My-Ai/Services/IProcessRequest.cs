namespace My_Ai.Services
{
    public interface IProcessRequest
    {

        public Task<string> ProcessWordDocumentAsync(IFormFile file, string prompt, string? additionalInfo = null);

        public Task<string> ProcessImageAsync(IFormFile file, string prompt, string? additionalInfo = null);
    }
}
