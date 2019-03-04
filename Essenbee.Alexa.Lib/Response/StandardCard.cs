using Essenbee.Alexa.Lib.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Essenbee.Alexa.Lib.Response
{
    public class StandardCard : ICard
    {
        [JsonRequired]
        [JsonProperty("type")]
        public string Type => "Standard";

        [JsonRequired]
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonRequired]
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore )]
        public CardImage Image { get; set; }
    }
}
