using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BusinessSearchTool.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using OfficeOpenXml;

namespace BusinessSearchTool;

[ApiController]
[Route("api/[controller]")]
public class BusinessSearchController : ControllerBase
{
    private readonly OpenAIService _openAiService;
    private readonly ExcelParserService _excelParserService;
    private readonly PromptBuilderService _promptBuilderService;

    public BusinessSearchController(OpenAIService openAiService , ExcelParserService excelParserService , PromptBuilderService promptBuilderService)
    {
        _openAiService = openAiService;
        _excelParserService = excelParserService;
        _promptBuilderService = promptBuilderService;
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
        //Initial file checks for early return
        if (request.File == null || request.File.Length == 0)
        {
            return BadRequest(new { error = "No file uploaded." });
        }
        if (!request.File.FileName.EndsWith(".xlsx"))
        {
            return BadRequest(new { error = "Only .xlsx files are supported." });
        }
        
        //Read file as stream
        using var stream = new MemoryStream();
        await request.File.CopyToAsync(stream);

        //Attempt to retrieve cache data if possible
        byte[] fileBytes = stream.ToArray();
        string hash = Cache.ComputeHash(fileBytes);
        if (Cache.FileHashCache.TryGetValue(hash, out var cachedResponse))
        {
            Console.WriteLine("✅ Cache hit for hash: " + hash);
            return Ok(new { ChartData = cachedResponse });
        }

        //Extract excel data into list of rows
        var data = _excelParserService.ExtractExcelData(stream);
        if (data is null)
        {
            return BadRequest(new { error = "Could not extract data from spreadsheet." });
        }

        //Craft payload
        var payloadText = _promptBuilderService.BuildPromptFromRawData(data);
        //Console.WriteLine(payloadText);
        
        //Wait for response from OpenAI
        var response = await SummarizePlainTextNoHTTP(payloadText.ToString());
        
        //TODO: Validate JSON data from OpenAI
        
        Cache.FileHashCache.TryAdd(hash, response);
        return Ok(new { ChartData = response });
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

    private async Task<string> SummarizePlainTextNoHTTP(string text)
    {
        try
        {
            var summary = await _openAiService.SummarizeText(text);
            return summary;  // Just return the string
        }
        catch (Exception ex)
        {
            // Optionally handle/log errors
            return null;  // Or throw if you want the calling method to handle it
        }
    }

    
}