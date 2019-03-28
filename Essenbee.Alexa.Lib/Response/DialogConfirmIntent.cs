using Essenbee.Alexa.Lib.Interfaces;
using Essenbee.Alexa.Lib.Request;
using Newtonsoft.Json;

namespace Essenbee.Alexa.Lib.Response
{
    public class DialogConfirmIntent : IDirective
    {
        [JsonProperty("type")]
        public string Type => "Dialog.ConfirmIntent";

        [JsonProperty("updatedIntent", NullValueHandling = NullValueHandling.Ignore)]
        public Intent UpdatedIntent { get; set; }
    }
}
