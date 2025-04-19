
using LacrmIntegration.Application.Common;
using LacrmIntegration.Application.Interfaces;

namespace LacrmIntegration.Application.Services
{
    public class InMemoryCallEventLogStore: ICallEventLogStore
    {
        private readonly List<CallEventLogEntry> _log = new();

        public void Add(CallEventLogEntry entry)
        {
            _log.Add(entry);
        }

        public List<CallEventLogEntry> GetAll()
        {
            return _log.OrderByDescending(x => x.Timestamp).ToList();
        }
    }
}
