using My_Ai.Util;
using System.Net.Mime;

namespace My_Ai.Services
{
    public class ProcessRequest : IProcessRequest
    {
        IClient _client;
        IImageClient _imageClient;
        public ProcessRequest(IClient client, IImageClient imageClient)
        {
            _client = client;
            _imageClient = imageClient;
        }

        public async Task<string> ProcessWordDocumentAsync(IFormFile file, string prompt, string? additionalInfo = null)
        {
            CheckFile(file);
            if (additionalInfo != null)
            {
                prompt = GeneratePrompt(prompt, additionalInfo);
            }
            try
            {
                var response = await _client.GenerateResponse(prompt, file);
                return !string.IsNullOrEmpty(response) ? response : "Error generating response from client.";
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating response from client.", ex);
            }
        }

        public async Task<string> ProcessTextAsync(string input, string prompt)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Input text is empty or not provided.");
            }

            prompt = GeneratePrompt(prompt, input);

            try
            {
                var response = await _client.GenerateResponse(prompt);
                return !string.IsNullOrEmpty(response) ? response : "Error generating response from client.";
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating response from client.", ex);
            }
        }

        public async Task<string> ProcessImageAsync(IFormFile file, string prompt, string? additionalInfo = null)
        {
            CheckFile(file);
            if (additionalInfo != null)
            {
                prompt = GeneratePrompt(prompt, additionalInfo);
            }
            try
            {
                var response = await _imageClient.GenerateResponse(file, prompt);
                return !string.IsNullOrEmpty(response) ? response : "Error generating response from client.";
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating response from client.", ex);
            }
        }

        private void CheckFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or not provided.");

            if (file.Length > 10 * 1024 * 1024) // 10 MB limit
                throw new ArgumentException("File size exceeds the limit of 10 MB.");

            // Reject legacy .doc early with a friendly message
            if (file.ContentType == "application/msword" ||
                Path.GetExtension(file.FileName).Equals(".doc", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Legacy .doc files are not supported. Please upload PDF, TXT, or DOCX.");
            }
        }

        private string GeneratePrompt(string prompt, string input)
        {
            var output = prompt.Replace("{0}", input);
            return output;
        }
    }
}
