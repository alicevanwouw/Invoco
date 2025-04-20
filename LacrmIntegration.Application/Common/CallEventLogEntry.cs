using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacrmIntegration.Application.Common
{
    public class CallEventLogEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; } 
        public string Endpoint { get; set; } = "/contacts/add";
        public int StatusCode { get; set; }     
        public string? ResponseMessage { get; set; } 
        public List<string>? Notes { get; set; }     
    }
}
