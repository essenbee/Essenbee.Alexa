using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Essenbee.Alexa.Lib.Request
{
    public class IntentRequest : RequestBody
    {
        [JsonProperty("dialogState")]
        public string DialogState { get; set; }

        [JsonProperty("intent")]
        public Intent Intent { get; set; }
    }
}
