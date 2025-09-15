using Bunit;
using My_Ai.Components.Pages.ArtsAndCrafts;

namespace My_Ai.BlazorTests.Components.Pages
{
    public class sewingPatternTests : TestBase
    {
        [Fact]
        public void sewingPattern_RendersCorrectly()
        {
            // Act
            var component = RenderComponent<sewingPattern>();

            // Assert - Check for main content that should be present
            Assert.Contains("Upload your image", component.Markup);
            Assert.Contains("Generate sewing Pattern", component.Markup);
        }

        [Fact]
        public void sewingPattern_HasFileUpload()
        {
            // Act
            var component = RenderComponent<sewingPattern>();

            // Assert
            var fileInput = component.Find("input[type='file']");
            Assert.NotNull(fileInput);
            Assert.Contains(".jpg,.png", fileInput.GetAttribute("accept"));
        }

        [Fact]
        public void sewingPattern_SubmitButtonInitiallyDisabled()
        {
            // Act
            var component = RenderComponent<sewingPattern>();

            // Assert
            var submitButton = component.Find("button");
            Assert.True(submitButton.HasAttribute("disabled"));
        }

        [Fact]
        public void sewingPattern_HasLoadingSpinnerComponent()
        {
            // Act
            var component = RenderComponent<sewingPattern>();

            // Assert - Look for main container structure
            Assert.Contains("container py-3", component.Markup);
        }

        [Fact]
        public void sewingPattern_DisplaysCorrectButtonText()
        {
            // Act
            var component = RenderComponent<sewingPattern>();

            // Assert
            Assert.Contains("Generate sewing Pattern", component.Markup);
        }

        [Fact]
        public void sewingPattern_HasBannerComponent()
        {
            // Act
            var component = RenderComponent<sewingPattern>();

            // Assert
            var banners = component.FindAll(".banner, section");
            Assert.True(banners.Count > 0);
        }
    }
}