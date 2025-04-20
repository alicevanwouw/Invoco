using LacrmIntegration.Application.Common;
using LacrmIntegration.Application.DTOs;
using LacrmIntegration.Application.Interfaces;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net;

namespace LacrmIntegrationAPI.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("api/CallEvent")]
    [ApiController]
    public class CallEventController : ControllerBase
    {
        private readonly ICallEventService _callEventService;
        private readonly LacrmSettings _settings;
        private readonly HttpClient _httpClient;

        public CallEventController(ICallEventService callEventService, LacrmSettings settings, HttpClient httpClient)
        {
            _callEventService = callEventService;
            _settings = settings;
            _httpClient = httpClient;
        }

        [HttpPost("callevent")]
        public async Task<IActionResult> HandleCallEvent([FromBody] CallEventDto dto)
        {
            var result = await _callEventService.HandleCallEventAsync(dto);
            return result.Success ? Ok(result) : StatusCode((int)result.StatusCode, result);
        }

        [HttpGet]
        public IActionResult GetCallLog()
        {
            var logEntries = _callEventService.GetCallLogs();
            return Ok(logEntries);
        }

        [HttpPut("{id}/note")]
        public async Task<IActionResult> UpdateNote(Guid id, [FromBody] CallEventNoteUpdateDto updateDto)
        {
            var success = await _callEventService.UpdateNoteAsync(id, updateDto.Note);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
