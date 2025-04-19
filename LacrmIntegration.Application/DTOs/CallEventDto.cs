
namespace LacrmIntegration.Application.DTOs
{
    public class CallEventDto
    {
        public required string EventName { get; set; }
        public DateTime CallStart { get; set; }
        public Guid CallId { get; set; }
        public required string CallersName { get; set; }
        public required string CallersTelephoneNumber { get; set; }
    }
}
