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
                        .Where(x => x.CallStart >= StartDate.Value && x.CallStart <= EndDate.Value)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch call logs.");
            }
        }

    }
}
