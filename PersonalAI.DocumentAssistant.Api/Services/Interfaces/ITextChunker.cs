using PersonalAI.DocumentAssistant.Api.Models;

namespace PersonalAI.DocumentAssistant.Api.Services.Interfaces;

public interface ITextChunker
{
    List<DocumentChunk> CreateChunks(
        List<PdfPageText> pages);
}