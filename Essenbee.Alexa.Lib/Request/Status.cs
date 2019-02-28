using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Essenbee.Alexa.Lib.Request
{
    public class Status
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
