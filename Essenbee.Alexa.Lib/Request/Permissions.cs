using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Essenbee.Alexa.Lib.Request
{
    public class Permissions
    {
        [JsonProperty("consentToken")]
        public string ConsentToken { get; set; }
    }
}
