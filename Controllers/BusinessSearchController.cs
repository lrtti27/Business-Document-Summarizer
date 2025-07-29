using Microsoft.AspNetCore.Mvc;

namespace BusinessSearchTool;

[ApiController]
[Route("api/[controller]")]
public class BusinessSearchController : ControllerBase
{
    private readonly OpenAIService _openAiService;

    public BusinessSearchController(OpenAIService openAiService)
    {
        _openAiService = openAiService;
    }
    
    [HttpPost("uploadText")]
    public async Task<IActionResult> UploadTextAndSummarize([FromBody] DocumentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return BadRequest(new { error = "Text cannot be empty." });
        }

        return await SummarizePlainText(request.Text);
    }

    [HttpPost("uploadFile")]
    public async Task<IActionResult> UploadFileAndSummarize([FromForm] DocumentUploadRequest request)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return BadRequest(new { error = "No file uploaded" });
        }
        //Extract the text from the document into plaintext
        string fileText;
        using (var reader = new StreamReader(request.File.OpenReadStream()))
        {
            fileText = await reader.ReadToEndAsync();
        }

        //Send request to openAI
        return await SummarizePlainText(fileText);
    }

    private async Task<IActionResult> SummarizePlainText(string text)
    {
        try
        {
            var summary = await _openAiService.SummarizeText(text);
            return Ok(new { Summary = summary });
        }
        catch (Exception ex)
        {
            // Log ex.Message or ex.StackTrace here
            return StatusCode(500, new { error = "Failed to summarize document.", details = ex.Message });
        }
    }
}