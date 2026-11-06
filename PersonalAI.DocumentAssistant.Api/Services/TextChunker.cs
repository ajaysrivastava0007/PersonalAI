using PersonalAI.DocumentAssistant.Api.Models;
using PersonalAI.DocumentAssistant.Api.Services.Interfaces;

namespace PersonalAI.DocumentAssistant.Api.Services;

public class TextChunker : ITextChunker
{
    private const int ChunkSize = 500;
    private const int ChunkOverlap = 50;

    public List<DocumentChunk> CreateChunks(
        List<PdfPageText> pages)
    {
        var chunks = new List<DocumentChunk>();

        int chunkIndex = 0;

        foreach (var page in pages)
        {
            var text = string.Join(
                " ",
                page.Text.Split(
                    (char[]?)null,
                    StringSplitOptions.RemoveEmptyEntries));

            int start = 0;

            while (start < text.Length)
            {
                int end = Math.Min(
                    start + ChunkSize,
                    text.Length);

                // Prefer breaking at a word boundary
                if (end < text.Length)
                {
                    int lastSpace = text.LastIndexOf(
                        ' ',
                        end - 1,
                        end - start);

                    if (lastSpace > start)
                        end = lastSpace;
                }

                var chunkText = text[start..end].Trim();

                if (!string.IsNullOrWhiteSpace(chunkText))
                {
                    chunks.Add(new DocumentChunk
                    {
                        ChunkIndex = chunkIndex++,
                        PageNumber = page.PageNumber,
                        Text = chunkText
                    });
                }

                if (end >= text.Length)
                    break;

                start = Math.Max(
                    start + 1,
                    end - ChunkOverlap);
            }
        }

        return chunks;
    }
}