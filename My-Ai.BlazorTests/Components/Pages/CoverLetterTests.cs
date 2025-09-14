using Bunit;
using My_Ai.Components.Pages.JobHunting;

namespace My_Ai.BlazorTests.Components.Pages
{
    public class CoverLetterTests : TestBase
    {
        [Fact]
        public void CoverLetter_RendersCorrectly()
        {
            // Act
            var component = RenderComponent<CoverLetter>();

            // Assert
            Assert.Contains("Job description", component.Markup);
            Assert.Contains("Upload your CV", component.Markup);
            Assert.Contains("Generate Cover Letter", component.Markup);
        }

        [Fact]
        public void CoverLetter_HasJobDescriptionTextArea()
        {
            // Act
            var component = RenderComponent<CoverLetter>();

            // Assert
            var textArea = component.Find("textarea[id='jobDesc']");
            Assert.NotNull(textArea);
            Assert.Contains("Paste the job description here...", textArea.GetAttribute("placeholder"));
        }

        [Fact]
        public void CoverLetter_HasFileUpload()
        {
            // Act
            var component = RenderComponent<CoverLetter>();

            // Assert
            var fileInput = component.Find("input[type='file']");
            Assert.NotNull(fileInput);
            Assert.Contains(".pdf,.docx,.txt", fileInput.GetAttribute("accept"));
        }

        [Fact]
        public void CoverLetter_SubmitButtonInitiallyDisabled()
        {
            // Act
            var component = RenderComponent<CoverLetter>();

            // Assert
            var submitButton = component.Find("button");
            Assert.True(submitButton.HasAttribute("disabled"));
        }

        [Fact]
        public void CoverLetter_HasLoadingSpinnerComponent()
        {
            // Act
            var component = RenderComponent<CoverLetter>();

            // Assert - Check for main container structure
            Assert.Contains("container py-3", component.Markup);
        }

        [Fact]
        public void CoverLetter_HasBannerComponent()
        {
            // Act
            var component = RenderComponent<CoverLetter>();

            // Assert
            var banners = component.FindAll(".banner, section");
            Assert.True(banners.Count > 0);
        }
    }
}