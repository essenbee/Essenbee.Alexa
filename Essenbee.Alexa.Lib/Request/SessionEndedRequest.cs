using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Essenbee.Alexa.Lib.Request
{
    public class SessionEndedRequest : RequestBody
    {
        [JsonProperty("reason")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Reason Reason { get; set; }
    }
}
