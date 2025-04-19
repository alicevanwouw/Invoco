using LacrmIntegration.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacrmIntegration.Application.Interfaces
{
    public interface ICallEventService
    {
        Task<CallEventDto> HandleCallEventAsync(CallEventDto callEvent);
    }
}
