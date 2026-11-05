namespace PersonalAI.DocumentAssistant.Api.Models;

public class PdfPageText
{
    public int PageNumber { get; set; }

    public string Text { get; set; } = string.Empty;
}