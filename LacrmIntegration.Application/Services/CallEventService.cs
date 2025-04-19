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
        public Task<CallEventDto> HandleCallEventAsync(CallEventDto callEvent)
        {
            throw new NotImplementedException();
        }
    }
}
