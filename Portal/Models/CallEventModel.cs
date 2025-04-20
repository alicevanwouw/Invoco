namespace Portal.Models
{
    public class CallEventModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; } 
        public required string Endpoint { get; set; }
        public int StatusCode { get; set; }     
        public string? ResponseMessage { get; set; }
        public string? Note { get; set; }     
    }
}
