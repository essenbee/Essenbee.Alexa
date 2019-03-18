using Newtonsoft.Json;

namespace Essenbee.Alexa.Lib.Response
{
    public class CardImage
    {
        [JsonProperty("smallImageUrl")]
        public string SmallImageUrl { get; set; }

        [JsonProperty("largeImageUrl")]
        public string LargeImageUrl { get; set; }

        public CardImage(string smallImageUrl, string largeImageUrl)
        {
            SmallImageUrl = smallImageUrl;
            LargeImageUrl = largeImageUrl;
        }
    }
}
