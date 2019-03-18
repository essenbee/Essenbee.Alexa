using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Essenbee.Alexa.Lib.Request
{
    public class Error
    {
        [JsonProperty("type")]
        public string ErrorType { get; set; }
        [JsonProperty("message")]
        public string ErrorMessage { get; set; }
    }
}
