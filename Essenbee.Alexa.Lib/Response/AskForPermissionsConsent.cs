using Essenbee.Alexa.Lib.Interfaces;
using Newtonsoft.Json;

namespace Essenbee.Alexa.Lib.Response
{
    public class AskForPermissionsConsent : ICard
    {
        [JsonRequired]
        [JsonProperty("type")]
        public string Type => "AskForPermissionsConsent";

        [JsonProperty("permissions")]
        public string[] Permissions { get; set; }

        public AskForPermissionsConsent(string[] permissions)
        {
            Permissions = permissions;
        }
    }
}
