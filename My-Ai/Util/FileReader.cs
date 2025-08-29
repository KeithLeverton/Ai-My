using Amazon.SimpleSystemsManagement;
using iText.Kernel.Pdf;
using DocumentFormat.OpenXml;
using System.Text;

namespace My_Ai.Util
{
    public static class FileReader
    {
        public static async Task<string> ReadPdfAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty or not provided.");
            }
            using (var stream = file.OpenReadStream())
            {
                using (var reader = new PdfReader(stream))
                {
                    using (var pdfDoc = new PdfDocument(reader))
                    {
                        var text = new StringBuilder();
                        for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                        {
                            var page = pdfDoc.GetPage(i);
                            var strategy = new iText.Kernel.Pdf.Canvas.Parser.Listener.SimpleTextExtractionStrategy();
                            var pageText = iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(page, strategy);
                            text.AppendLine(pageText);
                        }
                        return text.ToString();
                    }
                }
            }
        }

        public static async Task<string> ReadDocxAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty or not provided.");
            }
            DocumentFormat.OpenXml.Packaging.WordprocessingDocument wordDoc;
            using (var stream = file.OpenReadStream())
            {
                wordDoc = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(stream, false);
                var text = new StringBuilder();
                foreach (var paragraph in wordDoc.MainDocumentPart.Document.Body.Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
                {
                    text.AppendLine(paragraph.InnerText);
                }
                return text.ToString();
            }
        }
    }
}
