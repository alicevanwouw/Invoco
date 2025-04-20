using LacrmIntegration.Application.Common;
using LacrmIntegration.Application.DTOs;
using LacrmIntegration.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
