using Bunit;
using My_Ai.Components.Pages.JobHunting;

namespace My_Ai.BlazorTests.Components.Pages
{
    public class CustomiseCVTests : TestBase
    {
        [Fact]
        public void CustomiseCV_RendersCorrectly()
        {
            var component = RenderComponent<CustomiseCV>();
            Assert.Contains("Job description", component.Markup);
            Assert.Contains("Upload your CV", component.Markup);
            Assert.Contains("Customise your CV", component.Markup);
        }

        [Fact]
        public void CustomiseCV_HasFileUpload()
        {
            var component = RenderComponent<CustomiseCV>();
            var fileInput = component.Find("input[type='file']");
            Assert.NotNull(fileInput);
            Assert.Contains(".pdf,.docx,.txt", fileInput.GetAttribute("accept"));
        }

        [Fact]
        public void CustomiseCV_SubmitButtonInitiallyDisabled()
        {
            var component = RenderComponent<CustomiseCV>();
            var submitButton = component.Find("button");
            Assert.True(submitButton.HasAttribute("disabled"));
        }

        [Fact]
        public void CustomiseCV_HasInstructionAccordion()
        {
            var component = RenderComponent<CustomiseCV>();
            Assert.Contains("tool-instructions", component.Markup);
            Assert.Contains("faq-accordion", component.Markup);
            Assert.Contains("Instructions & Tips", component.Markup);
        }
    }
}
