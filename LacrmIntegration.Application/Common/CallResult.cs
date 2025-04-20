using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LacrmIntegration.Application.Common
{
    public class CallResult
    {
        public bool Success { get; set; }
        public required string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
