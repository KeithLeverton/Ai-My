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
            Assert.NotNull(Prompts.PaintByNumbers);
            Assert.NotNull(Prompts.SowingPrompt);
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
            Assert.Contains("coloring page", Prompts.PaintByNumbers);
            Assert.Contains("black-and-white", Prompts.PaintByNumbers);
            Assert.Contains("outlines", Prompts.PaintByNumbers);
        }

        [Fact]
        public void SowingPrompt_ContainsExpectedContent()
        {
            // Assert
            Assert.Contains("sewing pattern", Prompts.SowingPrompt);
            Assert.Contains("garment", Prompts.SowingPrompt);
            Assert.Contains("pattern pieces", Prompts.SowingPrompt);
        }
    }
}