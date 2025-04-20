using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal.Models;
using System.Net.Http;

namespace Portal.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;
        public List<CallEventModel> CallLogs { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? Endpoint { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? StatusCode { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? SortBy { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? SortDirection { get; set; } = "Asc"; // Default


        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _baseUrl = _configuration["ApiSettings:BaseUrl"] ?? throw new InvalidOperationException("No API BASE URL specified");
        }

        public async Task OnGetAsync()
        {
            if (!StartDate.HasValue)
                StartDate = DateTime.Now.AddDays(-1);

            if (!EndDate.HasValue)
                EndDate = DateTime.Now;

            try
            {
                var client = _httpClientFactory.CreateClient();                
                var apiUrl = $"{_baseUrl}/CallEvent"; 
                var result = await client.GetFromJsonAsync<List<CallEventModel>>(apiUrl);

                if (result != null)
                {
                    CallLogs = result;

                    CallLogs = CallLogs
                        .Where(x => x.Timestamp >= StartDate.Value && x.Timestamp <= EndDate.Value)
                        .ToList();

                    if (!string.IsNullOrWhiteSpace(Endpoint))
                    {
                        CallLogs = CallLogs
                            .Where(x => x.Endpoint?.Contains(Endpoint, StringComparison.OrdinalIgnoreCase) == true)
                            .ToList();
                    }

                    if (StatusCode.HasValue)
                    {
                        CallLogs = CallLogs
                            .Where(x => x.StatusCode == StatusCode.Value)
                            .ToList();
                    }

                    CallLogs = SortBy switch
                    {
                        "Timestamp" => SortDirection == "Desc" ? CallLogs.OrderByDescending(x => x.Timestamp).ToList() : CallLogs.OrderBy(x => x.Timestamp).ToList(),
                        "Endpoint" => SortDirection == "Desc" ? CallLogs.OrderByDescending(x => x.Endpoint).ToList() : CallLogs.OrderBy(x => x.Endpoint).ToList(),
                        "StatusCode" => SortDirection == "Desc" ? CallLogs.OrderByDescending(x => x.StatusCode).ToList() : CallLogs.OrderBy(x => x.StatusCode).ToList(),
                        _ => CallLogs
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch call logs.");
            }
        }

    }
}
