using System.Text;

namespace BusinessSearchTool.Services;

public class PromptBuilderService
{
    public string BuildPromptFromRawData(List<List<string>> data)
    {
        //Create plaintext representation of the data
        var payloadText = new StringBuilder();
        payloadText.AppendLine("Below is excel spreadsheet data given as all rows.");
        payloadText.AppendLine("Bear in mind that this includes ALL ROWS");
        payloadText.AppendLine("So a given row may contain all headers, such as example : ID , FirstName , LastName");
        payloadText.AppendLine("A given row may contain a row header followed by data, such as example : Revenue ,  100,000 , 200,000 , ...");
        payloadText.AppendLine("It is up to you to interpret this data in a way that is structured logically.");

        foreach (List<string> row in data)
        {
            payloadText.AppendLine("Row : ");
            foreach (string entry in row)
            {
                payloadText.AppendLine(entry);
            }
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

        return payloadText.ToString();
    }
}