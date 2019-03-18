using Essenbee.Alexa.Lib.Interfaces;
using Newtonsoft.Json;

namespace Essenbee.Alexa.Lib.Response
{
    public class SimpleCard : ICard
    {
        [JsonRequired]
        [JsonProperty("type")]
        public string Type => "Simple";

        [JsonRequired]
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonRequired]
        [JsonProperty("content")]
        public string Content { get; set; }

        public SimpleCard(string title, string text)
        {
            Title = title;
            Content = text;
        }
    }
}
