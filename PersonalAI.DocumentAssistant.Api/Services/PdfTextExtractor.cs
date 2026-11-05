using PersonalAI.DocumentAssistant.Api.Models;
using PersonalAI.DocumentAssistant.Api.Services.Interfaces;
using UglyToad.PdfPig;

namespace PersonalAI.DocumentAssistant.Api.Services;

public class PdfTextExtractor : IPdfTextExtractor
{
    public List<PdfPageText> ExtractText(string filePath)
    {
        var pages = new List<PdfPageText>();

        using var document = PdfDocument.Open(filePath);

        foreach (var page in document.GetPages())
        {
            pages.Add(new PdfPageText
            {
                PageNumber = page.Number,
                Text = page.Text
            });
        }

        return pages;
    }
}