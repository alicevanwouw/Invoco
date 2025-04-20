using LacrmIntegration.Application.Common;
using LacrmIntegration.Application.DTOs;
using LacrmIntegration.Application.Interfaces;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<LacrmClient> _logger;

        public CallEventService(ICallEventLogStore logStore, ILacrmClient lacrmClient, ILogger<LacrmClient> logger)
        {
            _logStore = logStore;
            _lacrmClient = lacrmClient;
            _logger = logger;
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
            var now = DateTime.UtcNow;
            var id = Guid.NewGuid();

            try
            {
                // Check for duplicates
                if (await _lacrmClient.ContactExistsAsync(dto.CallerTelephoneNumber))
                {
                    _logStore.Add(new CallEventLogEntry
                    {
                        Id = id,
                        Timestamp = now,
                        Endpoint = CallEventConstants.ContactsAddEndpoint,
                        StatusCode = 409,
                        ResponseMessage = CallEventConstants.DuplicateContactMessage,
                        Notes = new List<string>
                        {
                            $"Call started at {dto.CallStart:yyyy-MM-dd HH:mm:ss}",
                            $"Caller: {dto.CallerName}, Phone: {dto.CallerTelephoneNumber}"
                        }
                    });

                    return new CallResult
                    {
                        Success = false,
                        StatusCode = HttpStatusCode.Conflict,
                        Message = CallEventConstants.DuplicateContactMessage
                    };
                }

                // Try create contact
                var result = await _lacrmClient.CreateContactAsync(dto);

                _logStore.Add(new CallEventLogEntry
                {
                    Id = id,
                    Timestamp = now,
                    Endpoint = CallEventConstants.ContactsAddEndpoint,
                    StatusCode = (int)result.StatusCode,
                    ResponseMessage = result.Message,
                    Notes = new List<string>
                    {
                        $"Call from {dto.CallerName} at {dto.CallStart:yyyy-MM-dd HH:mm:ss} - Phone: {dto.CallerTelephoneNumber}"
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while handling call event for {Caller}", dto.CallerName);

                _logStore.Add(new CallEventLogEntry
                {
                    Id = id,
                    Timestamp = now,
                    Endpoint = CallEventConstants.ContactsAddEndpoint,
                    StatusCode = 500,
                    ResponseMessage = "Internal server error",
                    Notes = new List<string>
                    {
                        $"Call from {dto.CallerName} at {dto.CallStart:yyyy-MM-dd HH:mm:ss}",
                        $"Phone: {dto.CallerTelephoneNumber}",
                        $"Exception: {ex.Message}"
                    }
                });

                return new CallResult
                {
                    Success = false,
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "An unexpected error occurred while processing the call."
                };
            }
        }

        public bool AddNote(Guid id, string note)
        {
            var log = _logStore.GetAll().FirstOrDefault(x => x.Id == id);
            if (log == null)
                return false;

            log.Notes.Add(note);

            return true;
        }
    }
}
