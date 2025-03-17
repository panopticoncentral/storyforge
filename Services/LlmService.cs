using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StoryForge.Services;

public class LlmService
{
    private readonly HttpClient _httpClient;
    private readonly SettingsService _settingsService;
    private bool _useMockResponses = false;

    public LlmService()
    {
        _httpClient = new HttpClient();
        _settingsService = SettingsService.Instance;

        // Check if we have valid API settings, otherwise use mock responses
        var apiKey = _settingsService.GetApiKey();
        _useMockResponses = string.IsNullOrWhiteSpace(apiKey);

        if (!_useMockResponses)
        {
            UpdateApiSettings();
        }
    }

    public async Task<string> GetResponseAsync(string userMessage)
    {
        try
        {
            // Use mock responses if we don't have API settings configured
            if (_useMockResponses)
            {
                return MockResponse(userMessage);
            }

            var apiEndpoint = _settingsService.GetApiEndpoint();
            var model = _settingsService.GetSelectedModel();

            // Prepare the request based on the model type
            HttpContent content;
            if (model.StartsWith("claude"))
            {
                // Anthropic/Claude API format
                var requestData = new
                {
                    model = model,
                    messages = new[]
                    {
                        new { role = "user", content = userMessage }
                    },
                    max_tokens = 500
                };
                content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            }
            else if (model.StartsWith("gpt"))
            {
                // OpenAI API format
                var requestData = new
                {
                    model = model,
                    messages = new[]
                    {
                        new { role = "user", content = userMessage }
                    },
                    max_tokens = 500
                };
                content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            }
            else if (model.StartsWith("llama"))
            {
                // Generic API format for local or other LLMs
                var requestData = new
                {
                    model = model,
                    prompt = userMessage,
                    max_tokens = 500
                };
                content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            }
            else
            {
                // Default format
                var requestData = new
                {
                    model = model,
                    messages = new[]
                    {
                        new { role = "user", content = userMessage }
                    },
                    max_tokens = 500
                };
                content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.PostAsync(apiEndpoint, content);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var responseObject = JsonDocument.Parse(jsonResponse).RootElement;

            // Extract the response based on the API format
            if (model.StartsWith("claude"))
            {
                return ExtractClaudeResponse(responseObject);
            }
            else if (model.StartsWith("gpt"))
            {
                return ExtractOpenAIResponse(responseObject);
            }
            else if (model.StartsWith("llama"))
            {
                return ExtractGenericResponse(responseObject);
            }
            else
            {
                // Try various response formats
                try
                {
                    return ExtractOpenAIResponse(responseObject);
                }
                catch
                {
                    try
                    {
                        return ExtractClaudeResponse(responseObject);
                    }
                    catch
                    {
                        return ExtractGenericResponse(responseObject);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // If API call fails, fall back to mock responses but mention the error
            if (!_useMockResponses)
            {
                return $"Error communicating with LLM service: {ex.Message}. Please check your API settings.";
            }

            throw new Exception($"Error communicating with LLM service: {ex.Message}", ex);
        }
    }

    private string ExtractOpenAIResponse(JsonElement responseObject)
    {
        return responseObject.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString()
               ?? "No response content";
    }

    private string ExtractClaudeResponse(JsonElement responseObject)
    {
        return responseObject.GetProperty("content")[0].GetProperty("text").GetString()
               ?? "No response content";
    }

    private string ExtractGenericResponse(JsonElement responseObject)
    {
        // Try different response formats
        try
        {
            if (responseObject.TryGetProperty("text", out var textElement))
            {
                return textElement.GetString() ?? "No response content";
            }

            if (responseObject.TryGetProperty("output", out var outputElement))
            {
                return outputElement.GetString() ?? "No response content";
            }

            if (responseObject.TryGetProperty("response", out var responseElement))
            {
                return responseElement.GetString() ?? "No response content";
            }

            if (responseObject.TryGetProperty("result", out var resultElement))
            {
                return resultElement.GetString() ?? "No response content";
            }

            if (responseObject.TryGetProperty("message", out var messageElement))
            {
                return messageElement.GetString() ?? "No response content";
            }

            // Last resort, return the whole JSON as string
            return responseObject.ToString();
        }
        catch
        {
            return "Could not parse response from LLM service";
        }
    }

    // Update API settings when they change
    public void UpdateApiSettings()
    {
        var apiKey = _settingsService.GetApiKey();
        var model = _settingsService.GetSelectedModel();

        _useMockResponses = string.IsNullOrWhiteSpace(apiKey);

        if (!_useMockResponses)
        {
            _httpClient.DefaultRequestHeaders.Clear();

            // Set appropriate headers based on the model type
            if (model.StartsWith("claude"))
            {
                // Anthropic API
                _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
                _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
            }
            else if (model.StartsWith("gpt"))
            {
                // OpenAI API
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            }
            else
            {
                // Generic API with Bearer token as default
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            }
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