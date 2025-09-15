using Bunit;
using My_Ai.Components.Layout;

namespace My_Ai.BlazorTests.Components.Layout
{
    public class NavMenuTests : TestBase
    {
        [Fact]
        public void NavMenu_RendersCorrectly()
        {
            // Act
            var component = RenderComponent<NavMenu>();

            // Assert
            Assert.Contains("Ai-My", component.Markup);
            Assert.Contains("Job Apps", component.Markup);
            Assert.Contains("Arts and Crafts Apps", component.Markup);
        }

        [Fact]
        public void NavMenu_HasJobApplicationLinks()
        {
            // Act
            var component = RenderComponent<NavMenu>();

            // Assert - Check for the NavMenuLink component usage in the markup
            // Since NavMenuLink components are stubbed, check for their expected structure
            Assert.Contains("Review CV", component.Markup);
            Assert.Contains("Cover Letter", component.Markup);
            Assert.Contains("Customise CV", component.Markup);
        }

        [Fact]
        public void NavMenu_HasArtsAndCraftsLinks()
        {
            // Act
            var component = RenderComponent<NavMenu>();

            // Assert - Check for the text content of NavMenuLink components
            Assert.Contains("Colouring Page", component.Markup);
            Assert.Contains("sewing Pattern", component.Markup);
        }

        [Fact]
        public void NavMenu_HasNavMenuLinkComponents()
        {
            // Act
            var component = RenderComponent<NavMenu>();

            // Assert - Since NavMenuLink renders as anchor tags, check for navigation structure
            var navItems = component.FindAll(".nav-item");
            Assert.True(navItems.Count >= 2); // Should have at least 2 nav sections
        }

        [Fact]
        public void NavMenu_SectionsAreExpandable()
        {
            // Act
            var component = RenderComponent<NavMenu>();

            // Assert
            var details = component.FindAll("details");
            Assert.True(details.Count >= 2); // Should have at least 2 expandable sections
        }

        [Fact]
        public void NavMenu_HasBrandLink()
        {
            // Act
            var component = RenderComponent<NavMenu>();

            // Assert
            var brandLink = component.Find(".navbar-brand");
            Assert.NotNull(brandLink);
            Assert.Contains("Ai-My", brandLink.TextContent);
        }

        [Fact]
        public void NavMenu_HasCorrectEmojis()
        {
            // Act
            var component = RenderComponent<NavMenu>();

            // Assert - Check for emojis that should be rendered by NavMenuLink components
            Assert.Contains("📎", component.Markup); // Review CV
            Assert.Contains("✉️", component.Markup); // Cover Letter  
            Assert.Contains("📄", component.Markup); // Customise CV
            Assert.Contains("🖼️", component.Markup); // Colouring Page
            Assert.Contains("✂️", component.Markup); // sewing Pattern
        }

        [Fact]
        public void NavMenu_HasCorrectStructure()
        {
            // Act
            var component = RenderComponent<NavMenu>();

            // Assert - Check for the overall navigation structure
            Assert.Contains("navbar-brand", component.Markup);
            Assert.Contains("nav-scrollable", component.Markup);
            Assert.Contains("navbar-toggler", component.Markup);
        }
    }
}