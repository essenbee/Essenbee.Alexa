using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Essenbee.Alexa.Lib.Request
{
    public class Attributes
    {
        [JsonProperty("key")]
        public string Key { get; set; }
    }
}
