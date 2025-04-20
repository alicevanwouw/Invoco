
using LacrmIntegration.Application.Common;
using LacrmIntegration.Application.DTOs;

namespace LacrmIntegration.Application.Interfaces
{
    public interface ILacrmClient
    {
        Task<CallResult> CreateContactAsync(CallEventDto dto);
        Task<bool> ContactExistsAsync(string phoneNumber);
    }
}
