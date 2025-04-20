using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacrmIntegration.Application.Lacrm
{
    public class LacrmSettings
    {
        public required string BaseUrl { get; set; }
        public required string UserCode { get; set; }
        public required string ApiToken { get; set; }
    }
}
