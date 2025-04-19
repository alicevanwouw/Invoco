using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacrmIntegration.Application.Common
{
    public class CallEventLogEntry
    {
        public DateTime Timestamp { get; set; }
        public required string CallId { get; set; }
        public required string CallerName { get; set; }
        public required string PhoneNumber { get; set; }
        public DateTime CallStart { get; set; }
        public required string Status { get; set; }
        public required string ResponseMessage { get; set; }
    }
}
