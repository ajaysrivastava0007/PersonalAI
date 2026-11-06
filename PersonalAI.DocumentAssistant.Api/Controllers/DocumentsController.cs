using Microsoft.AspNetCore.Mvc;
using PersonalAI.DocumentAssistant.Api.Models;
using PersonalAI.DocumentAssistant.Api.Services;
using PersonalAI.DocumentAssistant.Api.Services.Interfaces;

namespace PersonalAI.DocumentAssistant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly IPdfTextExtractor _pdfTextExtractor;
    private readonly ITextChunker _textChunker;

    public DocumentsController(
        IWebHostEnvironment environment,
        IPdfTextExtractor pdfTextExtractor,
        ITextChunker textChunker)
    {
        _environment = environment;
        _pdfTextExtractor = pdfTextExtractor;
        _textChunker = textChunker;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Please upload a PDF.");

        if (!Path.GetExtension(file.FileName)
            .Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Only PDF files are allowed.");

        var uploadPath = Path.Combine(
            _environment.ContentRootPath,
            "Documents");

        Directory.CreateDirectory(uploadPath);

        //var fileName = $"{Guid.NewGuid()}.pdf";
        var fileName = file.FileName;
        var filePath = Path.Combine(uploadPath, fileName);

        await using var stream = new FileStream(
            filePath,
            FileMode.Create);

        await file.CopyToAsync(stream);

        return Ok(new DocumentUploadResponse
        {
            FileName = file.FileName,
            FilePath = filePath,
            FileSize = file.Length,
            Message = "PDF uploaded successfully."
        });
    }

    [HttpGet("{fileName}/extract")]
    public IActionResult ExtractText(string fileName)
    {
        if (Path.GetFileName(fileName) != fileName ||
            !fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Invalid PDF file name.");

        var filePath = Path.Combine(
            _environment.ContentRootPath,
            "Documents",
            fileName);

        if (!System.IO.File.Exists(filePath))
            return NotFound("PDF document not found.");

        var pages = _pdfTextExtractor.ExtractText(filePath);

        return Ok(new
        {
            FileName = fileName,
            PageCount = pages.Count,
            Pages = pages
        });
    }

    [HttpGet("{fileName}/chunks")]
    public IActionResult GetChunks(string fileName)
    {
        if (Path.GetFileName(fileName) != fileName ||
            !fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Invalid PDF file name.");

        var filePath = Path.Combine(
            _environment.ContentRootPath,
            "Documents",
            fileName);

        if (!System.IO.File.Exists(filePath))
            return NotFound("PDF document not found.");

        var pages = _pdfTextExtractor.ExtractText(filePath);

        var chunks = _textChunker.CreateChunks(pages);

        return Ok(new
        {
            FileName = fileName,
            TotalChunks = chunks.Count,
            Chunks = chunks
        });
    }
}