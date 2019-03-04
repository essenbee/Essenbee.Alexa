using Essenbee.Alexa.Lib.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Essenbee.Alexa.Lib.Response
{
    public class TextOutput : ISpeech
    {
        [JsonRequired]
        [JsonProperty("type")]
        public string Type => "PlainText";

        [JsonRequired]
        [JsonProperty("text")]
        public string Text { get; set; }

        public TextOutput(string text)
        {
            Text = text;
        }
    }
}
