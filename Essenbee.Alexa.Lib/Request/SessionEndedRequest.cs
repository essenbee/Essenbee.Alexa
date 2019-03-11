using Newtonsoft.Json;

namespace Essenbee.Alexa.Lib.Request
{
    public class SessionEndedRequest : RequestBody
    {
        [JsonProperty("reason")]
        public string Reason { get; set; }
    }
}
