using Essenbee.Alexa.Lib.Interfaces;
using Essenbee.Alexa.Lib.Request;
using Newtonsoft.Json;

namespace Essenbee.Alexa.Lib.Response
{
    public class DialogConfirmSlot : IDirective
    {
        [JsonProperty("type")]
        public string Type => "Dialog.ConfirmSlot";

        [JsonProperty("slotToConfirm"), JsonRequired]
        public string SlotToConfirm { get; set; }

        [JsonProperty("updatedIntent", NullValueHandling = NullValueHandling.Ignore)]
        public Intent UpdatedIntent { get; set; }

        public DialogConfirmSlot(string slotToConfirm)
        {
            SlotToConfirm = slotToConfirm;
        }
    }
}
