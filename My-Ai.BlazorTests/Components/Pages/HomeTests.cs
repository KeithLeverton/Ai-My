using Bunit;
using My_Ai.Components.Pages;

namespace My_Ai.BlazorTests.Components.Pages
{
    public class HomeTests : TestBase
    {
        [Fact]
        public void Home_RendersCorrectly()
        {
            // Act
            var component = RenderComponent<Home>();

            // Assert
            Assert.Contains("Welcome to Ai-My", component.Markup);
            Assert.Contains("Choose a tile to get started", component.Markup);
        }

        [Fact]
        public void Home_HasJobApplicationsSection()
        {
            // Act
            var component = RenderComponent<Home>();

            // Assert
            Assert.Contains("Job Applications", component.Markup);
            Assert.Contains("Review your CV", component.Markup);
            Assert.Contains("Cover Letter", component.Markup);
            Assert.Contains("Customise cv", component.Markup);
        }

        [Fact]
        public void Home_HasArtsAndCraftsSection()
        {
            // Act
            var component = RenderComponent<Home>();

            // Assert
            Assert.Contains("Arts and Crafts Applications", component.Markup);
            Assert.Contains("Colouring page", component.Markup);
            Assert.Contains("Sowing Pattern", component.Markup);
        }

        [Fact]
        public void Home_HasCorrectLinks()
        {
            // Act
            var component = RenderComponent<Home>();

            // Assert - Note: In Home.razor, there are two links pointing to ColouringPage
            var reviewCvLink = component.Find("a[href='/ReviewCV']");
            var coverLetterLink = component.Find("a[href='/CoverLetter']");
            var customiseCvLink = component.Find("a[href='/CustomiseCV']");
            var colouringPageLinks = component.FindAll("a[href='/ColouringPage']");

            Assert.NotNull(reviewCvLink);
            Assert.NotNull(coverLetterLink);
            Assert.NotNull(customiseCvLink);
            Assert.True(colouringPageLinks.Count >= 1); // At least one ColouringPage link
        }

        [Fact]
        public void Home_TilesHaveCorrectIcons()
        {
            // Act
            var component = RenderComponent<Home>();

            // Assert
            Assert.Contains("📎", component.Markup); // Review CV
            Assert.Contains("✉️", component.Markup); // Cover Letter
            Assert.Contains("📄", component.Markup); // Customise CV
            Assert.Contains("🖼️", component.Markup); // Colouring Page
            Assert.Contains("✂️", component.Markup); // Sowing Pattern
        }

        [Fact]
        public void Home_HasExpandableSections()
        {
            // Act
            var component = RenderComponent<Home>();

            // Assert
            var jobSection = component.Find(".job-applications-section");
            var artsSection = component.Find(".arts-and-crafts-section");
            
            Assert.NotNull(jobSection);
            Assert.NotNull(artsSection);
        }
    }
}