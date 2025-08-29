namespace My_Ai.Util
{
    public class HtmlConverter
    {
        public static string ReWriteHtml(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            string html = text;

            // First, handle bullet points (lines starting with * followed by space)
            html = System.Text.RegularExpressions.Regex.Replace(
                html,
                @"^\*\s+(.*)$",
                "<li>$1</li>",
                System.Text.RegularExpressions.RegexOptions.Multiline);

            // Convert **text** to <strong> tags (for bold text within content)
            html = System.Text.RegularExpressions.Regex.Replace(
                html,
                @"\*\*(.*?)\*\*",
                "<strong>$1</strong>");

            // Convert remaining *text* to <em> tags (for italic/emphasis)
            html = System.Text.RegularExpressions.Regex.Replace(
                html,
                @"(?<!\*)\*([^*]+?)\*(?!\*)",
                "<em>$1</em>");

            // Replace newlines with <br/>
            html = html.Replace("\n", "<br/>");

            // Wrap bullet points in <ul> if any <li> exists
            if (html.Contains("<li>"))
            {
                // More sophisticated wrapping: group consecutive <li> items
                html = System.Text.RegularExpressions.Regex.Replace(
                    html,
                    @"(<li>.*?</li>(?:<br/>)?)+",
                    "<ul>$0</ul>",
                    System.Text.RegularExpressions.RegexOptions.Singleline);
                
                // Clean up extra <br/> tags around lists
                html = System.Text.RegularExpressions.Regex.Replace(
                    html,
                    @"<br/><ul>|</ul><br/>",
                    m => m.Value.Contains("<ul>") ? "<ul>" : "</ul>");
            }

            return html;
        }
    }
}
