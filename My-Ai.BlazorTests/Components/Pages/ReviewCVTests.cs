using Bunit;
using My_Ai.Components.Pages.JobHunting;

namespace My_Ai.BlazorTests.Components.Pages
{
    public class ReviewCVTests : TestBase
    {
        [Fact]
        public void ReviewCV_RendersCorrectly()
        {
            var component = RenderComponent<ReviewCV>();
            Assert.Contains("Upload your CV", component.Markup);
            Assert.Contains("Review your CV", component.Markup);
        }

        [Fact]
        public void ReviewCV_HasFileUpload()
        {
            var component = RenderComponent<ReviewCV>();
            var fileInput = component.Find("input[type='file']");
            Assert.NotNull(fileInput);
            Assert.Contains(".pdf,.docx,.txt", fileInput.GetAttribute("accept"));
        }

        [Fact]
        public void ReviewCV_SubmitButtonInitiallyDisabled()
        {
            var component = RenderComponent<ReviewCV>();
            var submitButton = component.Find("button");
            Assert.True(submitButton.HasAttribute("disabled"));
        }

        [Fact]
        public void ReviewCV_HasInstructionAccordion()
        {
            var component = RenderComponent<ReviewCV>();
            Assert.Contains("tool-instructions", component.Markup);
            Assert.Contains("faq-accordion", component.Markup);
            Assert.Contains("Instructions & Tips", component.Markup);
        }
    }
}
