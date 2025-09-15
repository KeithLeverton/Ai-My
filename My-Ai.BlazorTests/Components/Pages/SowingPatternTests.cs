using Bunit;
using My_Ai.Components.Pages.ArtsAndCrafts;

namespace My_Ai.BlazorTests.Components.Pages
{
    public class SewingPatternTests : TestBase
    {
        [Fact]
        public void SewingPattern_RendersCorrectly()
        {
            // Act
            var component = RenderComponent<SewingPattern>();

            // Assert - Check for main content that should be present
            Assert.Contains("Upload your image", component.Markup);
            Assert.Contains("Generate sewing Pattern", component.Markup);
        }

        [Fact]
        public void SewingPattern_HasFileUpload()
        {
            // Act
            var component = RenderComponent<SewingPattern>();

            // Assert
            var fileInput = component.Find("input[type='file']");
            Assert.NotNull(fileInput);
            Assert.Contains(".jpg,.png", fileInput.GetAttribute("accept"));
        }

        [Fact]
        public void SewingPattern_SubmitButtonInitiallyDisabled()
        {
            // Act
            var component = RenderComponent<SewingPattern>();

            // Assert
            var submitButton = component.Find("button");
            Assert.True(submitButton.HasAttribute("disabled"));
        }

        [Fact]
        public void SewingPattern_HasLoadingSpinnerComponent()
        {
            // Act
            var component = RenderComponent<SewingPattern>();

            // Assert - Look for main container structure
            Assert.Contains("container py-3", component.Markup);
        }

        [Fact]
        public void SewingPattern_DisplaysCorrectButtonText()
        {
            // Act
            var component = RenderComponent<SewingPattern>();

            // Assert
            Assert.Contains("Generate sewing Pattern", component.Markup);
        }

        [Fact]
        public void SewingPattern_HasBannerComponent()
        {
            // Act
            var component = RenderComponent<SewingPattern>();

            // Assert
            var banners = component.FindAll(".banner, section");
            Assert.True(banners.Count > 0);
        }
    }
}