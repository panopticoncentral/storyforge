using System;
using System.Threading.Tasks;

namespace StoryForge.Services;

public class LlmService
{
    //private readonly HttpClient _httpClient;

    //private readonly string _apiEndpoint = "https://api.example.com/v1/chat";
    //private readonly string _apiKey = "YOUR_API_KEY";

    //public LlmService()
    //{
        //_httpClient = new HttpClient();
        //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    //}

    public async Task<string> GetResponseAsync(string userMessage)
    {
        try
        {
            // TODO: For initial testing, return a mock response
            // Remove this when ready to connect to an actual LLM API
            return MockResponse(userMessage);

            /* Uncomment this section when ready to connect to a real LLM API

            var requestData = new
            {
                messages = new[]
                {
                    new { role = "user", content = userMessage }
                },
                model = "YOUR_MODEL_NAME",
                max_tokens = 500
            };

            var jsonContent = JsonSerializer.Serialize(requestData);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_apiEndpoint, httpContent);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

            return responseObject.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "No response";
            */
        }
        catch (Exception ex)
        {
            throw new Exception($"Error communicating with LLM service: {ex.Message}", ex);
        }
    }

    private static string MockResponse(string userMessage)
    {
        if (userMessage.Contains("hello", StringComparison.InvariantCultureIgnoreCase) 
            || userMessage.Contains("hi", StringComparison.InvariantCultureIgnoreCase))
        {
            return "Hello there! How can I assist you today?";
        }

        if (userMessage.EndsWith("?", StringComparison.InvariantCultureIgnoreCase))
        {
            return "That's an interesting question. When we connect to a real LLM, I'll be able to provide a more thoughtful answer.";
        }

        return $"I received your message. This is a placeholder response until we connect to an actual LLM API. You said: \"{userMessage}\"";
    }
}