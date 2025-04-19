using LacrmIntegration.Application.DTOs;
using LacrmIntegration.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LacrmIntegrationAPI.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("api/CallEvent")]
    [ApiController]
    public class CallEventController : ControllerBase
    {
        private readonly ICallEventService _callEventService;

        public CallEventController(ICallEventService callEventService)
        {
            _callEventService = callEventService;
        }

        [HttpPost]
        public async Task<IActionResult> PostCallEvent([FromBody] CallEventDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _callEventService.HandleCallEventAsync(dto);

            if (!result.Success)
                return StatusCode((int)result.StatusCode, result.Message);

            return Ok(result.Message);
        }
    }
}
