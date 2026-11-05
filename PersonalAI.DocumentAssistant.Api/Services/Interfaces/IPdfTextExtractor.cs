using PersonalAI.DocumentAssistant.Api.Models;

namespace PersonalAI.DocumentAssistant.Api.Services.Interfaces;

public interface IPdfTextExtractor
{
    List<PdfPageText> ExtractText(string filePath);
}