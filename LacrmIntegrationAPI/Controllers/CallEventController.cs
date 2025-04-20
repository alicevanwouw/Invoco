using LacrmIntegration.Application.Common;
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
        public async Task<CallResult> HandleCallEventAsync(CallEventDto dto)
        {
            //var contactPayload = MapToLacrmContact(dto); 
            //var note = $"Call from {dto.CallerName} at {dto.CallStart:yyyy-MM-dd HH:mm}. Number: {dto.CallerTelephoneNumber}";

            //var result = await _lacrmApiClient.CreateContactAsync(contactPayload);

            //var logEntry = new CallEventLogEntry
            //{
            //    Timestamp = DateTime.UtcNow,
            //    Endpoint = "/contacts/add",
            //    StatusCode = (int)result.StatusCode,
            //    ResponseMessage = result.Message,
            //    Note = note
            //};

            //_callEventService.Add(logEntry); // or whatever you named it

            return null;
        }

        [HttpGet]
        public IActionResult GetCallLog()
        {
            var logEntries = _callEventService.GetCallLogs();
            return Ok(logEntries);
        }

        [HttpPut("{id}/note")]
        public IActionResult UpdateNote(Guid id, [FromBody] CallEventNoteUpdateDto updateDto)
        {
            var log = _callEventService.GetCallLogs().FirstOrDefault(x => x.Id == id);
            if (log == null)
                return NotFound();

            log.Note = updateDto.Note;

            return NoContent(); 
        }
    }
}
