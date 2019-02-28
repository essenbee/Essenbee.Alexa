using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Essenbee.Alexa.Lib.Request
{
    public class RequestBody
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
