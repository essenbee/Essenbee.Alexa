using Essenbee.Alexa.Lib.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Essenbee.Alexa.Lib.Response
{
    public class SsmlSpeech : ISpeech
    {
        [JsonRequired]
        [JsonProperty("type")]
        public string Type => "SSML";

        [JsonRequired]
        [JsonProperty("ssml")]
        public string Ssml { get; set; }
    }
}
