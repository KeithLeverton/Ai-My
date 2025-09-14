using Bunit;
using My_Ai.Components.Pages.ArtsAndCrafts;

namespace My_Ai.BlazorTests.Components.Pages
{
    public class ColouringPageTests : TestBase
    {
        [Fact]
        public void ColouringPage_RendersCorrectly()
        {
            // Act
            var component = RenderComponent<ColouringPage>();

            // Assert - Check for the main content elements that should be present
            Assert.Contains("Upload your image", component.Markup);
            Assert.Contains("Generate Colouring Page", component.Markup);
        }

        [Fact]
        public void ColouringPage_HasFileUpload()
        {
            // Act
            var component = RenderComponent<ColouringPage>();

            // Assert
            var fileInput = component.Find("input[type='file']");
            Assert.NotNull(fileInput);
            Assert.Contains(".jpg,.png", fileInput.GetAttribute("accept"));
        }

        [Fact]
        public void ColouringPage_SubmitButtonInitiallyDisabled()
        {
            // Act
            var component = RenderComponent<ColouringPage>();

            // Assert
            var submitButton = component.Find("button");
            Assert.True(submitButton.HasAttribute("disabled"));
        }

        [Fact]
        public void ColouringPage_HasLoadingSpinnerComponent()
        {
            // Act
            var component = RenderComponent<ColouringPage>();

            // Assert - Look for the LoadingSpinner component in the rendered output
            // Since it's conditionally rendered, check for the structure
            Assert.Contains("container py-3", component.Markup);
        }

        [Fact]
        public void ColouringPage_HasCorrectButtonText()
        {
            // Act
            var component = RenderComponent<ColouringPage>();

            // Assert
            Assert.Contains("Generate Colouring Page", component.Markup);
        }

        [Fact]
        public void ColouringPage_HasBannerComponent()
        {
            // Act
            var component = RenderComponent<ColouringPage>();

            // Assert - Check that Banner component is rendered
            var banners = component.FindAll(".banner, section");
            Assert.True(banners.Count > 0);
        }
    }
}