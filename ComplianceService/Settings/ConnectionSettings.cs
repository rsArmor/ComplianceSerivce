using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComplianceService.Settings
{
    public class ConnectionSettings
    {
       public string FedsfmUrl { get; set; }
       public string BankApiUrl { get; set; }
       public string ClientId { get; set; }
       public string SertificatePassword { get; set; }
    }
}
