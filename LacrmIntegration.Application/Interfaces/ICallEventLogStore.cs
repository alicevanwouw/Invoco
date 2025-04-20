

using LacrmIntegration.Application.Common;

namespace LacrmIntegration.Application.Interfaces
{
    public interface ICallEventLogStore
    {
        void Add(CallEventLogEntry entry);
        List<CallEventLogEntry> GetAll();
    }
}
