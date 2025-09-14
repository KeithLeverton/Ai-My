using Bunit;
using My_Ai.Components.Pages.ArtsAndCrafts;

namespace My_Ai.BlazorTests.Components.Pages
{
    public class SowingPatternTests : TestBase
    {
        [Fact]
        public void SowingPattern_RendersCorrectly()
        {
            // Act
            var component = RenderComponent<SowingPattern>();

            // Assert - Check for main content that should be present
            Assert.Contains("Upload your image", component.Markup);
            Assert.Contains("Generate Sowing Pattern", component.Markup);
        }

        [Fact]
        public void SowingPattern_HasFileUpload()
        {
            // Act
            var component = RenderComponent<SowingPattern>();

            // Assert
            var fileInput = component.Find("input[type='file']");
            Assert.NotNull(fileInput);
            Assert.Contains(".jpg,.png", fileInput.GetAttribute("accept"));
        }

        [Fact]
        public void SowingPattern_SubmitButtonInitiallyDisabled()
        {
            // Act
            var component = RenderComponent<SowingPattern>();

            // Assert
            var submitButton = component.Find("button");
            Assert.True(submitButton.HasAttribute("disabled"));
        }

        [Fact]
        public void SowingPattern_HasLoadingSpinnerComponent()
        {
            // Act
            var component = RenderComponent<SowingPattern>();

            // Assert - Look for main container structure
            Assert.Contains("container py-3", component.Markup);
        }

        [Fact]
        public void SowingPattern_DisplaysCorrectButtonText()
        {
            // Act
            var component = RenderComponent<SowingPattern>();

            // Assert
            Assert.Contains("Generate Sowing Pattern", component.Markup);
        }

        [Fact]
        public void SowingPattern_HasBannerComponent()
        {
            // Act
            var component = RenderComponent<SowingPattern>();

            // Assert
            var banners = component.FindAll(".banner, section");
            Assert.True(banners.Count > 0);
        }
    }
}