using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal.Models;

namespace Portal.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public List<CallEventModel> CallLogs { get; set; } = new();

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var apiUrl = "https://localhost:5001/api/callevent";//temporary change to actual when everything is connected 

                var result = await client.GetFromJsonAsync<List<CallEventModel>>(apiUrl);

                if (result != null)
                    CallLogs = result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch call logs.");
            }
        }
    }
}
