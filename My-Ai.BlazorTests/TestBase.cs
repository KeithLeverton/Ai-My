using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Moq;
using My_Ai.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;

namespace My_Ai.BlazorTests
{
    public abstract class TestBase : TestContext
    {
        protected Mock<IProcessRequest> MockProcessRequest { get; }
        protected Mock<IConfiguration> MockConfiguration { get; }

        protected TestBase()
        {
            // Setup mocks
            MockProcessRequest = new Mock<IProcessRequest>();
            MockConfiguration = new Mock<IConfiguration>();
            
            // Configure mock configuration for any config dependencies
            MockConfiguration.Setup(x => x["ChatGPTApiKey"]).Returns("test-api-key");
            MockConfiguration.Setup(x => x["GeminiApiKey"]).Returns("test-gemini-key");
            MockConfiguration.Setup(x => x["RequestTimeoutSeconds"]).Returns("30");
            
            // Register services that components might need
            Services.AddSingleton(MockProcessRequest.Object);
            Services.AddSingleton(MockConfiguration.Object);
            
            // Add JSInterop mock for Blazor components that might need it
            var jsRuntime = new Mock<IJSRuntime>();
            Services.AddSingleton(jsRuntime.Object);
            
            // Add routing services for NavMenuLink components
            Services.AddSingleton<NavigationManager>(new MockNavigationManager("https://localhost:5001/"));
            
            // Only stub external dependencies that might cause issues
            // Let NavMenuLink render normally since we want to test navigation
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Clean up any resources if needed
            }
            base.Dispose(disposing);
        }
    }

    // Mock NavigationManager for testing
    public class MockNavigationManager : NavigationManager
    {
        public MockNavigationManager(string baseUri) : base()
        {
            Initialize(baseUri, baseUri);
        }

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            // Do nothing for tests
        }
    }
}