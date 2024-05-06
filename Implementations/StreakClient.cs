using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SwimTaykaStreak
{
    public class StreakClient : IStreakClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly ILogger<StreakClient> _logger;

        public StreakClient(IConfiguration configuration, ILogger<StreakClient> logger)
        {
            _baseUrl = configuration["Streak:BaseUrl"];

            _logger = logger;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<string>> GetBoxKeysAsync(string streakKeyApi, string boxKey)
        {
            string url = $"{_baseUrl}pipelines/{boxKey}/boxes";

            // Configure the authorization header using a secure method to retrieve API keys
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", streakKeyApi);

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var boxes = ExtractKeysFromJson(jsonString);
                return boxes;
            }

            return null;
        }

        public async Task<string> GetBoxUrlAsync(string streakKeyApi, string boxKey, string fieldId)
        {
            try
            {
                // Build the URL for the specific field of the box
                string fieldUrl = $"{_baseUrl}boxes/{boxKey}/fields/{fieldId}";

                // Configure the authorization header using a secure method to retrieve API keys
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", streakKeyApi);

                var response = await _httpClient.GetAsync(fieldUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    return ExtractUrlFromJson(jsonResponse);
                }
                else
                {
                    throw new Exception($"Failed to retrieve the field: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Proper error handling here
                _logger?.LogError($"Error occurred: {ex.Message}");
                return null;
            }
        }

        public async Task<string> PostStreakField(string streakKeyApi, string boxKey, string value, string fieldToUpdateId)
        {
            var url = $"{_baseUrl}boxes/{boxKey}/fields/{fieldToUpdateId}";
            var requestBody = new { value = value };
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            try
            {
                // Configure the authorization header using a secure method to retrieve API keys
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", streakKeyApi);

                using var response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode(); // This will throw if not successful

                return await response.Content.ReadAsStringAsync(); // Return the response body from server
            }
            catch (Exception ex)
            {
                _logger?.LogError($"Exception caught while posting to Streak: {ex}");
                throw; // Rethrow the exception to be handled further up the call chain
            }
        }


        private List<string> ExtractKeysFromJson(string jsonString)
        {
            List<string> keys = new List<string>();
            using (JsonDocument document = JsonDocument.Parse(jsonString))
            {
                ExtractKeys(document.RootElement, keys);
            }
            return keys;
        }

        // Recursive method to extract values of 'key' from any JSON element
        private void ExtractKeys(JsonElement element, List<string> keys)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (JsonProperty property in element.EnumerateObject())
                    {
                        // Check if the property name is 'key' and add it to the list
                        if (property.Name.Equals("key", StringComparison.OrdinalIgnoreCase))
                        {
                            if (property.Value.ValueKind == JsonValueKind.String)
                            {
                                keys.Add(property.Value.GetString());
                            }
                        }
                        // Recurse into the object or arrays within
                        ExtractKeys(property.Value, keys);
                    }
                    break;
                case JsonValueKind.Array:
                    foreach (JsonElement item in element.EnumerateArray())
                    {
                        ExtractKeys(item, keys);
                    }
                    break;
            }
        }

        private string ExtractUrlFromJson(string jsonString)
        {
            var jsonDoc = JsonDocument.Parse(jsonString);
            if (jsonDoc.RootElement.TryGetProperty("value", out JsonElement valueElement) && valueElement.ValueKind == JsonValueKind.String)
            {
                _logger?.LogDebug($"Url: {valueElement.GetString()}");
                return valueElement.GetString();
            }
            return string.Empty;
        }
    }
}