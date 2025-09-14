using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using My_Ai.Services;
using System.Text;

namespace My_Ai.Tests.Services
{
    public class ProcessRequestTests
    {
        private readonly Mock<IClient> _mockClient;
        private readonly Mock<IImageClient> _mockImageClient;
        private readonly ProcessRequest _service;

        public ProcessRequestTests()
        {
            _mockClient = new Mock<IClient>();
            _mockImageClient = new Mock<IImageClient>();
            _service = new ProcessRequest(_mockClient.Object, _mockImageClient.Object);
        }

        [Fact]
        public async Task ProcessWordDocumentAsync_ValidFile_ReturnsResponse()
        {
            // Arrange
            var mockFile = CreateMockFormFile("test.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Test content");
            var prompt = "Test prompt";
            var expectedResponse = "Test response";
            
            _mockClient.Setup(x => x.GenerateResponse(It.IsAny<string>(), It.IsAny<IFormFile>()))
                      .ReturnsAsync(expectedResponse);

            // Act
            var result = await _service.ProcessWordDocumentAsync(mockFile, prompt);

            // Assert
            Assert.Equal(expectedResponse, result);
            _mockClient.Verify(x => x.GenerateResponse(prompt, mockFile), Times.Once);
        }

        [Fact]
        public async Task ProcessWordDocumentAsync_WithAdditionalInfo_ReplacesPlaceholder()
        {
            // Arrange
            var mockFile = CreateMockFormFile("test.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Test content");
            var prompt = "Test prompt {0}";
            var additionalInfo = "additional";
            var expectedPrompt = "Test prompt additional";
            var expectedResponse = "Test response";
            
            _mockClient.Setup(x => x.GenerateResponse(expectedPrompt, It.IsAny<IFormFile>()))
                      .ReturnsAsync(expectedResponse);

            // Act
            var result = await _service.ProcessWordDocumentAsync(mockFile, prompt, additionalInfo);

            // Assert
            Assert.Equal(expectedResponse, result);
            _mockClient.Verify(x => x.GenerateResponse(expectedPrompt, mockFile), Times.Once);
        }

        [Fact]
        public async Task ProcessImageAsync_ValidFile_ReturnsResponse()
        {
            // Arrange
            var mockFile = CreateMockFormFile("test.jpg", "image/jpeg", "Test image content");
            var prompt = "Test prompt";
            var expectedResponse = "Test response";
            
            _mockImageClient.Setup(x => x.GenerateResponse(It.IsAny<IFormFile>(), It.IsAny<string>()))
                           .ReturnsAsync(expectedResponse);

            // Act
            var result = await _service.ProcessImageAsync(mockFile, prompt);

            // Assert
            Assert.Equal(expectedResponse, result);
            _mockImageClient.Verify(x => x.GenerateResponse(mockFile, prompt), Times.Once);
        }

        [Theory]
        [InlineData(null, "File is empty or not provided.")]
        [InlineData("", "File is empty or not provided.")]
        public async Task ProcessWordDocumentAsync_EmptyFile_ThrowsArgumentException(string? content, string expectedMessage)
        {
            // Arrange
            var mockFile = content == null ? null : CreateMockFormFile("test.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", content);
            var prompt = "Test prompt";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _service.ProcessWordDocumentAsync(mockFile, prompt));
            Assert.Contains(expectedMessage, exception.Message);
        }

        [Fact]
        public async Task ProcessWordDocumentAsync_FileTooLarge_ThrowsArgumentException()
        {
            // Arrange
            var largeContent = new string('x', 11 * 1024 * 1024); // 11MB
            var mockFile = CreateMockFormFile("test.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", largeContent);
            var prompt = "Test prompt";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _service.ProcessWordDocumentAsync(mockFile, prompt));
            Assert.Contains("File size exceeds the limit", exception.Message);
        }

        [Fact]
        public async Task ProcessWordDocumentAsync_LegacyDocFile_ThrowsArgumentException()
        {
            // Arrange
            var mockFile = CreateMockFormFile("test.doc", "application/msword", "Test content");
            var prompt = "Test prompt";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _service.ProcessWordDocumentAsync(mockFile, prompt));
            Assert.Contains("Legacy .doc files are not supported", exception.Message);
        }

        [Fact]
        public async Task ProcessTextAsync_EmptyInput_ThrowsArgumentException()
        {
            // Arrange
            var input = "";
            var prompt = "Test prompt";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _service.ProcessTextAsync(input, prompt));
            Assert.Contains("Input text is empty or not provided", exception.Message);
        }

        [Fact]
        public async Task ProcessTextAsync_ValidInput_ReturnsResponse()
        {
            // Arrange
            var input = "Test input";
            var prompt = "Test prompt {0}";
            var expectedPrompt = "Test prompt Test input";
            var expectedResponse = "Test response";
            
            _mockClient.Setup(x => x.GenerateResponse(expectedPrompt))
                      .ReturnsAsync(expectedResponse);

            // Act
            var result = await _service.ProcessTextAsync(input, prompt);

            // Assert
            Assert.Equal(expectedResponse, result);
            _mockClient.Verify(x => x.GenerateResponse(expectedPrompt), Times.Once);
        }

        [Fact]
        public async Task ProcessWordDocumentAsync_ClientThrowsException_WrapsException()
        {
            // Arrange
            var mockFile = CreateMockFormFile("test.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Test content");
            var prompt = "Test prompt";
            var innerException = new InvalidOperationException("Inner exception");
            
            _mockClient.Setup(x => x.GenerateResponse(It.IsAny<string>(), It.IsAny<IFormFile>()))
                      .ThrowsAsync(innerException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => 
                _service.ProcessWordDocumentAsync(mockFile, prompt));
            Assert.Contains("Error generating response from client", exception.Message);
            Assert.Equal(innerException, exception.InnerException);
        }

        private static IFormFile CreateMockFormFile(string fileName, string contentType, string content)
        {
            if (content == null)
            {
                var nullMock = new Mock<IFormFile>();
                nullMock.Setup(f => f.Length).Returns(0);
                return nullMock.Object;
            }

            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            var mock = new Mock<IFormFile>();
            
            mock.Setup(f => f.FileName).Returns(fileName);
            mock.Setup(f => f.ContentType).Returns(contentType);
            mock.Setup(f => f.Length).Returns(bytes.Length);
            mock.Setup(f => f.OpenReadStream()).Returns(stream);
            mock.Setup(f => f.Headers).Returns(new HeaderDictionary());
            
            return mock.Object;
        }
    }
}