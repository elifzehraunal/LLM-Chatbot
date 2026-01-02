using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization; // Requires System.Web.Extensions

namespace LLM_Chatbot.Services
{
    public class GeminiService : ILLMService
    {
        private readonly string _apiKey;
        // Switching to 'gemini-pro-latest' as 'gemini-pro' was not found (2026 environment)
        private readonly string _endpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro-latest:generateContent";

        public GeminiService(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<string> GetResponseAsync(string message)
        {
            if (string.IsNullOrEmpty(_apiKey) || _apiKey == "YOUR_API_KEY_HERE")
            {
                return "Error: API Key is missing. Please configure it in the source code (GeminiService.cs).";
            }

            using (var client = new HttpClient())
            {
                var requestUrl = $"{_endpoint}?key={_apiKey}";

                // Simple JSON construction to avoid heavy dependencies if possible, 
                // but using JavaScriptSerializer is cleaner if available in .NET Framework.
                // Structure for Gemini: { "contents": [{ "parts": [{ "text": "..." }] }] }

                var payload = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = message }
                            }
                        }
                    }
                };

                var serializer = new JavaScriptSerializer();
                var jsonContent = serializer.Serialize(payload);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                try
                {
                    // Console.WriteLine for debugging only
                    // Console.WriteLine($"Sending: {jsonContent}");

                    var response = await client.PostAsync(requestUrl, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorBody = await response.Content.ReadAsStringAsync();
                        return $"Error: {response.StatusCode} - {errorBody}";
                    }

                    var responseString = await response.Content.ReadAsStringAsync();

                    // Parse response
                    // Expected: { "candidates": [ { "content": { "parts": [ { "text": "..." } ] } } ] }
                    dynamic result = serializer.Deserialize<dynamic>(responseString);

                    try
                    {
                        string responseText = result["candidates"][0]["content"]["parts"][0]["text"];
                        return responseText;
                    }
                    catch
                    {
                        return "Error: Unexpected response format.";
                    }
                }
                catch (Exception ex)
                {
                    return $"Exception: {ex.Message}";
                }
            }
        }
    }
}
