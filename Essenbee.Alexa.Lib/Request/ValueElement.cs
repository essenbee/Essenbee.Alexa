using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Essenbee.Alexa.Lib.Request
{
    public class ValueElement
    {
        [JsonProperty("value")]
        public Value Value { get; set; }
    }
}
