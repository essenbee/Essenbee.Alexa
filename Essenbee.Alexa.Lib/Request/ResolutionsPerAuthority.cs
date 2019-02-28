using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Essenbee.Alexa.Lib.Request
{
    public class ResolutionsPerAuthority
    {
        [JsonProperty("authority")]
        public string Authority { get; set; }

        [JsonProperty("status")]
        public Status Status { get; set; }

        [JsonProperty("values")]
        public ValueElement[] Values { get; set; }
    }
}
