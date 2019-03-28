using Essenbee.Alexa.Lib.Interfaces;
using Essenbee.Alexa.Lib.Request;
using Newtonsoft.Json;

namespace Essenbee.Alexa.Lib.Response
{
    public class DialogDelegate : IDirective
    {
        [JsonProperty("type")]
        public string Type => "Dialog.Delegate";

        [JsonProperty("updatedIntent", NullValueHandling = NullValueHandling.Ignore)]
        public Intent UpdatedIntent { get; set; }
    }
}
