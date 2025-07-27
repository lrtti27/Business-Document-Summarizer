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
    
    [HttpPost("summarize")]
    public async Task<IActionResult> SummarizeDocument([FromBody] DocumentRequest request)
    {
        
        try
        {
            var summary = await _openAiService.SummarizeText(request.Text);
            return Ok(new { Summary = summary });
        }
        catch (Exception ex)
        {
            // Log ex.Message or ex.StackTrace here
            return StatusCode(500, new { error = "Failed to summarize document.", details = ex.Message });
        }
    }
}