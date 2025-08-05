using System.Text.Json;

namespace BusinessSearchTool;

public class OpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAIService(string apiKey)
    {
        _apiKey = apiKey;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
    }

    public async Task<string> SummarizeText(string text)
    {
        //Anonymous object used to form JSON body
        var requestBody = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "system", content = "Summarize this document." },
                new { role = "user", content = text }
            }
        };
        //Send POST to OpenAI
        //Serialize data into JSON
        //Return Task<HttpResponseMessage>
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);
        response.EnsureSuccessStatusCode();
        
        //Response Content is of type HTTPContent , represents body of HTTP response
        //Reads response body as JSON , deserializes into JSON element
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        return result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
    }
}