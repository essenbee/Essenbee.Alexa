using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Essenbee.Alexa.Lib.Request
{
    public class SystemClass
    {
        [JsonProperty("device")]
        public Device Device { get; set; }

        [JsonProperty("application")]
        public Application Application { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("apiEndpoint")]
        public Uri ApiEndpoint { get; set; }

        [JsonProperty("apiAccessToken")]
        public string ApiAccessToken { get; set; }
    }
}
