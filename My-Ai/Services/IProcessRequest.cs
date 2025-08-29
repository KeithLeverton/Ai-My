namespace My_Ai.Services
{
    public interface IProcessRequest
    {

        public Task<string> ProcessFileAsync(IFormFile file, string prompt, string? additionalInfo = null);
    }
}
