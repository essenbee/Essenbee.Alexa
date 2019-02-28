using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Essenbee.Alexa.Lib.Request
{
    public class Context
    {
        [JsonProperty("System")]
        public SystemClass System { get; set; }

        [JsonProperty("AudioPlayer")]
        public ContextAudioPlayer AudioPlayer { get; set; }
    }
}
