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

        public CallEventService(ICallEventLogStore logStore)
        {
            _logStore = logStore;
        }

        public List<CallEventLogEntry> GetCallLogs()
        {
            return _logStore.GetAll();
        }

        public Task<CallResult> HandleCallEventAsync(CallEventDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
