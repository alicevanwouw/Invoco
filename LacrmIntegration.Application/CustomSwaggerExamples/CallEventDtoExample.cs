using LacrmIntegration.Application.DTOs;
using Swashbuckle.AspNetCore.Filters;

public class CallEventDtoExample : IExamplesProvider<CallEventDto>
{
    public CallEventDto GetExamples()
    {
        return new CallEventDto
        {
            CallerName = "Alice Test",
            CallerTelephoneNumber = "01527306999",
            CallStart = DateTime.UtcNow,
            EventName = "CallLog",
        };
    }
}