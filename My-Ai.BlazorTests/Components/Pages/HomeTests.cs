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
            Assert.Contains("Choose a service below to get started", component.Markup);
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
            Assert.Contains("Customise CV", component.Markup); // Fixed: Changed from "Customise cv" to "Customise CV"
        }

        [Fact]
        public void Home_HasArtsAndCraftsSection()
        {
            // Act
            var component = RenderComponent<Home>();

            // Assert
            Assert.Contains("Arts and Crafts Applications", component.Markup);
            Assert.Contains("Colouring Page", component.Markup); // Fixed: Changed from "Colouring page" to "Colouring Page"
            Assert.Contains("Sewing Pattern", component.Markup); // Fixed: Changed from "sewing Pattern" to "Sewing Pattern"
        }

        [Fact]
        public void Home_HasCorrectLinks()
        {
            // Act
            var component = RenderComponent<Home>();

            // Assert
            var reviewCvLink = component.Find("a[href='/ReviewCV']");
            var coverLetterLink = component.Find("a[href='/CoverLetter']");
            var customiseCvLink = component.Find("a[href='/CustomiseCV']");
            var colouringPageLink = component.Find("a[href='/ColouringPage']");
            var sewingPatternLink = component.Find("a[href='/SewingPattern']"); // Fixed: Added missing sewing pattern link

            Assert.NotNull(reviewCvLink);
            Assert.NotNull(coverLetterLink);
            Assert.NotNull(customiseCvLink);
            Assert.NotNull(colouringPageLink);
            Assert.NotNull(sewingPatternLink);
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
            Assert.Contains("✂️", component.Markup); // Sewing Pattern
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

        [Fact]
        public void Home_HasAccessibilityFeatures()
        {
            // Act
            var component = RenderComponent<Home>();

            // Assert - Check for accessibility improvements
            Assert.Contains("role=\"button\"", component.Markup);
            Assert.Contains("aria-expanded", component.Markup);
            Assert.Contains("aria-describedby", component.Markup);
        }

        [Fact]
        public void Home_HasSemanticStructure()
        {
            // Act
            var component = RenderComponent<Home>();

            // Assert - Check for semantic HTML structure
            Assert.Contains("<header", component.Markup);
            Assert.Contains("<section", component.Markup);
            Assert.Contains("services-section", component.Markup);
        }
    }
}