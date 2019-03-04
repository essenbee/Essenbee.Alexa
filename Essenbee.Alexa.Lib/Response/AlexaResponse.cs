using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Essenbee.Alexa.Lib.Response
{
    public class AlexaResponse
    {
        [JsonRequired]
        [JsonProperty("version")]
        public string Version => "1.0";

        [JsonProperty("sessionAttributes", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> SessionAttributes { get; set;}

        [JsonRequired]
        [JsonProperty("response")]
        public ResponseBody Response { get; set; }
    }
}
