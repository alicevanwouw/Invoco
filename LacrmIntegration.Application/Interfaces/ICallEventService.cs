using LacrmIntegration.Application.Common;
using LacrmIntegration.Application.DTOs;

namespace LacrmIntegration.Application.Interfaces
{
    public interface ICallEventService
    {
        Task<CallResult> HandleCallEventAsync(CallEventDto dto);
        List<CallEventLogEntry> GetCallLogs();
        void Add(CallEventLogEntry entry);
        Task<bool> UpdateNoteAsync(Guid id, string note);
    }
}
