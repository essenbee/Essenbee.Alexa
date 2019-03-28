using Essenbee.Alexa.Lib.Interfaces;
using Essenbee.Alexa.Lib.Request;
using Newtonsoft.Json;

namespace Essenbee.Alexa.Lib.Response
{
    public class DialogElicitSlot : IDirective
    {
        [JsonProperty("type")]
        public string Type => "Dialog.ElicitSlot";

        [JsonProperty("slotToElicit"), JsonRequired]
        public string SlotToElict { get; set; }

        [JsonProperty("updatedIntent", NullValueHandling = NullValueHandling.Ignore)]
        public Intent UpdatedIntent { get; set; }

        public DialogElicitSlot(string slotToElicit)
        {
            SlotToElict = slotToElicit;
        }
    }
}
