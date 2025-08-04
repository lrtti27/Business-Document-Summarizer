using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using OfficeOpenXml;

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
            return BadRequest(new { error = "No file uploaded." });
        }

        if (!request.File.FileName.EndsWith(".xlsx"))
        {
            return BadRequest(new { error = "Only .xlsx files are supported." });
        }

        using var stream = new MemoryStream();
        await request.File.CopyToAsync(stream);

        using var package = new ExcelPackage(stream);
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();

        if (worksheet == null)
        {
            return BadRequest(new { error = "No worksheet found." });
        }

        int rowCount = worksheet.Dimension.Rows;
        int colCount = worksheet.Dimension.Columns;

        var data = new List<List<string>>();
        for (int row = 1; row <= rowCount; row++)
        {
            var rowData = new List<string>();
            for (int col = 1; col <= colCount; col++)
            {
                rowData.Add(worksheet.Cells[row, col].Text);
            }

            data.Add(rowData);
        }

        var financialData = new Dictionary<string, Dictionary<string, string>>();

        //Populate the dictionary with all row vals in first column of data
        for (int row = 1; row < data.Count; row++)
        {
            string rowKey = data[row][0];
            var rowDict = new Dictionary<string, string>();
            for (int col = 1; col < data[0].Count; col++)
            {
                string header = data[0][col]; // e.g., "2021"
                string value = data[row][col]; // e.g., "1000000"
                rowDict.Add(header, value); // ✅ key is the year now
            }

            financialData.Add(rowKey, rowDict);
        }


        //Create plaintext representation of the data
        var payloadText = new StringBuilder();
        payloadText.AppendLine("Below is financial data.");
        foreach ((string metric, Dictionary<string, string> metricValues) in financialData)
        {
            payloadText.AppendLine($"Metric: {metric}");
            foreach ((string point, string value) in metricValues)
            {
                payloadText.AppendLine($"- {point}: {value}");
            }

            payloadText.AppendLine();
        }

        payloadText.AppendLine("Based on this data, generate JSON formatted output for chart.js.");
        payloadText.AppendLine("The format should be:");
        payloadText.AppendLine(@"
{
  ""labels"": [""2021"", ""2022"", ""2023""],
  ""datasets"": [
    {
      ""label"": ""Example Label 1"",
      ""data"": [1000000, 1200000, 1350000]
      ""backgroundColor"": ""rgba(75, 192, 192, 0.2)"",
      ""borderColor"": ""rgba(75, 192, 192, 1)""
    },
    {
      ""label"": ""Example Label 2"",
      ""data"": [300000, 400000, 500000]
      ""backgroundColor"": ""rgba(255, 99, 132, 0.2)"",
      ""borderColor"": ""rgba(255, 99, 132, 1)""
    }
  ]
}
");
        payloadText.AppendLine("Add your own lines DO NOT SIMPLY USE WHAT IS IN THE SUGGESTED FORMAT ABOVE!!!! OR ELSE BAD THINGS HAPPEN");
        payloadText.AppendLine("The suggested format above is nothing more than that -- a suggestion...");
        payloadText.AppendLine("Respond ONLY with valid JSON.");

        Console.WriteLine(payloadText);
        var response = await SummarizePlainTextNoHTTP(payloadText.ToString());
        
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