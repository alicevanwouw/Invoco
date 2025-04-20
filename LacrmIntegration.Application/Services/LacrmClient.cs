using LacrmIntegration.Application.Common;
using LacrmIntegration.Application.DTOs;
using LacrmIntegration.Application.Interfaces;
using System.Net.Http.Json;

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
        var requestPayload = new
        {
            IsCompany = false,
            Name = dto.CallerName,
            Phone = new[]
            {
                new { Text = dto.CallerTelephoneNumber, Type = "Mobile" }
            }
        };

        var url = $"{_settings.BaseUrl}CreateContact?UserCode={_settings.UserCode}&ApiToken={_settings.ApiToken}";

        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, requestPayload);
            var responseContent = await response.Content.ReadAsStringAsync();

            return new CallResult
            {
                Success = response.IsSuccessStatusCode,
                StatusCode = response.StatusCode,
                Message = responseContent
            };
        }
        catch (Exception ex)
        {
            return new CallResult
            {
                Success = false,
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Message = $"Exception occurred: {ex.Message}"
            };
        }
    }
}
