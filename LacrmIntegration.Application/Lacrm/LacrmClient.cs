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
using Microsoft.Extensions.Logging;
using LacrmIntegration.Application.Lacrm;

public class LacrmClient : ILacrmClient
{
    private readonly HttpClient _httpClient;
    private readonly LacrmSettings _settings;
    private readonly ILogger<LacrmClient> _logger;

    public LacrmClient(HttpClient httpClient, LacrmSettings settings, ILogger<LacrmClient> logger)
    {
        _httpClient = httpClient;
        _settings = settings;
        _logger = logger;
    }

    public async Task<CallResult> CreateContactAsync(CallEventDto dto)
    {
        var userId = await GetUserIdAsync(); 
        if (string.IsNullOrWhiteSpace(userId))
        {
            _logger.LogWarning("Failed to retrieve LACRM UserId for contact creation.");
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

            _logger.LogInformation("CreateContact LACRM response status: {StatusCode}", response.StatusCode);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to create contact. Response: {Content}", content);
            }

            return new CallResult
            {
                Success = response.IsSuccessStatusCode,
                StatusCode = response.StatusCode,
                Message = content
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while creating contact in LACRM.");
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
        _logger.LogInformation("Checking if contact exists for phone number: {Phone}", phoneNumber);
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
            var content = await response.Content.ReadAsStringAsync();     

            if (!response.IsSuccessStatusCode)
                return false;

            var json = JObject.Parse(content);
            var results = json["Results"] as JArray;

            _logger.LogInformation("ContactExistsAsync LACRM response status: {StatusCode}", response.StatusCode);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to retrieve contact. Response: {Content}", content);
            }

            return results != null && results.Any();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while retreiving contact in LACRM.");
            return false;
        }
    }

    public async Task<string?> GetUserIdAsync()
    {
        try
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
        catch (Exception ex)
        {
             _logger.LogError(ex, "Failed to retrieve LACRM user ID.");
            return null;
        }
    }

}
