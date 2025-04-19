using LacrmIntegration.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LacrmIntegrationAPI.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("api/Alert")]
    [ApiController]
    public class CallEventController : ControllerBase
    {
        private readonly ICallEventService _callEventService;

        public CallEventController(ICallEventService callEventService)
        {
            _callEventService = callEventService;
        }
    }
}
