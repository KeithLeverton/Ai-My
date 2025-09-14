using My_Ai.Util;

namespace My_Ai.Tests.Util
{
    public class HtmlConverterTests
    {
        [Theory]
        [InlineData("", "")]
        [InlineData(null, "")]
        public void ReWriteHtml_EmptyOrNullInput_ReturnsEmpty(string input, string expected)
        {
            // Act
            var result = HtmlConverter.ReWriteHtml(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ReWriteHtml_BulletPoints_ConvertsToListItems()
        {
            // Arrange
            var input = "* First item\n* Second item\n* Third item";

            // Act
            var result = HtmlConverter.ReWriteHtml(input);

            // Assert
            Assert.Contains("<ul>", result);
            Assert.Contains("<li>First item</li>", result);
            Assert.Contains("<li>Second item</li>", result);
            Assert.Contains("<li>Third item</li>", result);
            Assert.Contains("</ul>", result);
        }

        [Fact]
        public void ReWriteHtml_BoldText_ConvertsToBoldTags()
        {
            // Arrange
            var input = "This is **bold text** in a sentence.";

            // Act
            var result = HtmlConverter.ReWriteHtml(input);

            // Assert
            Assert.Contains("<strong>bold text</strong>", result);
        }

        [Fact]
        public void ReWriteHtml_ItalicText_ConvertsToEmphasisTags()
        {
            // Arrange
            var input = "This is *italic text* in a sentence.";

            // Act
            var result = HtmlConverter.ReWriteHtml(input);

            // Assert
            Assert.Contains("<em>italic text</em>", result);
        }

        [Fact]
        public void ReWriteHtml_Newlines_ConvertsToBrTags()
        {
            // Arrange
            var input = "Line 1\nLine 2\nLine 3";

            // Act
            var result = HtmlConverter.ReWriteHtml(input);

            // Assert
            Assert.Contains("Line 1<br/>", result);
            Assert.Contains("Line 2<br/>", result);
            Assert.Contains("Line 3", result);
        }

        [Fact]
        public void ReWriteHtml_MixedFormatting_HandlesAllFormats()
        {
            // Arrange
            var input = "**Bold text**\n* First item\n* Second item with *italic*\nNormal text";

            // Act
            var result = HtmlConverter.ReWriteHtml(input);

            // Assert
            Assert.Contains("<strong>Bold text</strong>", result);
            Assert.Contains("<ul>", result);
            Assert.Contains("<li>First item</li>", result);
            Assert.Contains("<li>Second item with <em>italic</em></li>", result);
            Assert.Contains("</ul>", result);
            Assert.Contains("<br/>", result);
        }
    }
}