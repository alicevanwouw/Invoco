using LacrmIntegration.Application.Common;
using LacrmIntegration.Application.DTOs;
using LacrmIntegration.Application.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;

public class LacrmClient : ILacrmClient
{
    private readonly HttpClient _httpClient;
    private readonly LacrmSettings _settings;

    public LacrmClient(HttpClient httpClient, LacrmSettings settings)
    {
        _httpClient = httpClient;
        _settings = settings;
    }

    public async Task<CallResult> CreateContactAsync(CallEventDto dto)
    {
        var userId = await GetUserIdAsync(); // 👈 Get the AssignedTo ID
        if (string.IsNullOrWhiteSpace(userId))
        {
            return new CallResult
            {
                Success = false,
                StatusCode = HttpStatusCode.Unauthorized,
                Message = "Could not retrieve LACRM user ID"
            };
        }

        var requestPayload = new
        {
            Function = "CreateContact",
            Parameters = new
            {
                IsCompany = false,
                AssignedTo = userId,
                Name = dto.CallerName,
                Phone = new[]
                {
                new { Text = dto.CallerTelephoneNumber, Type = "Mobile" }
            }
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, _settings.BaseUrl)
        {
            Content = new StringContent(JsonConvert.SerializeObject(requestPayload), Encoding.UTF8, "application/json")
        };

        request.Headers.Add("Authorization", _settings.ApiToken);

        try
        {
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            return new CallResult
            {
                Success = response.IsSuccessStatusCode,
                StatusCode = response.StatusCode,
                Message = content
            };
        }
        catch (Exception ex)
        {
            return new CallResult
            {
                Success = false,
                StatusCode = HttpStatusCode.InternalServerError,
                Message = $"Exception occurred: {ex.Message}"
            };
        }
    }



    public async Task<bool> ContactExistsAsync(string phoneNumber)
    {
        var requestBody = new
        {
            Function = "GetContacts",
            Parameters = new
            {
                SearchTerms = phoneNumber,
                MaxNumberOfResults = 1
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, _settings.BaseUrl)
        {
            Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json")
        };

        request.Headers.Add("Authorization", _settings.ApiToken); 

        try
        {
            var response = await _httpClient.SendAsync(request);
            var rawContent = await response.Content.ReadAsStringAsync();     

            if (!response.IsSuccessStatusCode)
                return false;

            var json = JObject.Parse(rawContent);
            var results = json["Results"] as JArray;

            return results != null && results.Any();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"EXCEPTION: {ex.Message}");
            return false;
        }
    }

    public async Task<string?> GetUserIdAsync()
    {
        var body = new
        {
            Function = "GetUser",
            Parameters = new { }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, _settings.BaseUrl)
        {
            Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")
        };

        request.Headers.Add("Authorization", _settings.ApiToken);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            return null;

        var json = JObject.Parse(await response.Content.ReadAsStringAsync());
        return json["UserId"]?.ToString();
    }

}
