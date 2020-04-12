using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ComplianceService.Models.Fedsfm
{
    public partial class TerroristResponse
    {
            [JsonProperty("query")]
            public string Query { get; set; }

            [JsonProperty("suggestions")]
            public List<string> Suggestions { get; set; }     
    }
}
