
namespace LacrmIntegration.Application.DTOs
{
    public class CallEventDto
    {
        public required string EventName { get; set; }
        public DateTime CallStart { get; set; }
        public Guid CallId { get; set; }
        public required string CallerName { get; set; }
        public required string CallerTelephoneNumber { get; set; }
    }
}
