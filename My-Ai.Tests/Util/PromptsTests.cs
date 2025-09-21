using My_Ai.Util;

namespace My_Ai.Tests.Util
{
    public class PromptsTests
    {
        [Fact]
        public void Prompts_ContainsExpectedPrompts()
        {
            // Assert
            Assert.NotNull(Prompts.ImproveCv);
            Assert.NotNull(Prompts.CustomiseCv);
            Assert.NotNull(Prompts.GenerateCoverLetter);
            Assert.NotNull(Prompts.ColouringPage);
            Assert.NotNull(Prompts.SewingPrompt);
        }

        [Fact]
        public void ImproveCv_ContainsExpectedContent()
        {
            // Assert
            Assert.Contains("CV", Prompts.ImproveCv);
            Assert.Contains("expert", Prompts.ImproveCv);
        }

        [Fact]
        public void CustomiseCv_ContainsPlaceholder()
        {
            // Assert
            Assert.Contains("{0}", Prompts.CustomiseCv);
            Assert.Contains("job description", Prompts.CustomiseCv);
        }

        [Fact]
        public void GenerateCoverLetter_ContainsPlaceholder()
        {
            // Assert
            Assert.Contains("{0}", Prompts.GenerateCoverLetter);
            Assert.Contains("cover letter", Prompts.GenerateCoverLetter);
        }

        [Fact]
        public void PaintByNumbers_ContainsExpectedContent()
        {
            // Assert - Updated to match actual content
            Assert.Contains("coloring page", Prompts.ColouringPage);
            Assert.Contains("black-and-white", Prompts.ColouringPage);
            Assert.Contains("outlines", Prompts.ColouringPage);
        }

        [Fact]
        public void sewingPrompt_ContainsExpectedContent()
        {
            // Assert
            Assert.Contains("sewing pattern", Prompts.SewingPrompt);
            Assert.Contains("garment", Prompts.SewingPrompt);
            Assert.Contains("pattern pieces", Prompts.SewingPrompt);
        }
    }
}