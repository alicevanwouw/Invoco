using LacrmIntegration.Application.Common;
using LacrmIntegration.Application.DTOs;
using LacrmIntegration.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LacrmIntegration.Application.Services
{
    public class CallEventService : ICallEventService
    {
        private readonly ICallEventLogStore _logStore;
        private readonly ILacrmClient _lacrmClient;

        public CallEventService(ICallEventLogStore logStore, ILacrmClient lacrmClient)
        {
            _logStore = logStore;
            _lacrmClient = lacrmClient;
        }

        public void Add(CallEventLogEntry entry)
        {
            _logStore.Add(entry);
        }

        public List<CallEventLogEntry> GetCallLogs()
        {
            return _logStore.GetAll();
        }

        public async Task<CallResult> HandleCallEventAsync(CallEventDto dto)
        {
            if (await _lacrmClient.ContactExistsAsync(dto.CallerTelephoneNumber))
            {
                var log = new CallEventLogEntry
                {
                    Id = Guid.NewGuid(),
                    Timestamp = DateTime.UtcNow,
                    Endpoint = CallEventConstants.ContactsAddEndpoint,
                    StatusCode = 409,
                    ResponseMessage = CallEventConstants.DuplicateContactMessage,
                    Notes = new List<string>
                    {
                        $"Call started at {dto.CallStart:yyyy-MM-dd HH:mm:ss}",
                        $"Caller: {dto.CallerName}, Phone: {dto.CallerTelephoneNumber}"
                    }
                };

                _logStore.Add(log);

                return new CallResult
                {
                    Success = false,
                    StatusCode = HttpStatusCode.Conflict,
                    Message = "Duplicate contact"
                };
            }

            var result = await _lacrmClient.CreateContactAsync(dto);

            var logEntry = new CallEventLogEntry
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                Endpoint = "/contacts/add",
                StatusCode = (int)result.StatusCode,
                ResponseMessage = result.Message,
                Notes = new List<string>
            {
                $"Call from {dto.CallerName} at {dto.CallStart} - Phone: {dto.CallerTelephoneNumber}"
            }
            };

            _logStore.Add(logEntry);

            return result;
        }

        public async Task<bool> UpdateNoteAsync(Guid id, string note)
        {
            var log = _logStore.GetAll().FirstOrDefault(x => x.Id == id);
            if (log == null)
                return false;

            log.Notes.Add(note);

            return true;
        }
    }
}
