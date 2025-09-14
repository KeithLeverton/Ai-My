namespace My_Ai.Services
{
    public interface IImageClient 
    {
        public Task<string> GenerateResponse(IFormFile inputImage, string prompt);
    }
}
