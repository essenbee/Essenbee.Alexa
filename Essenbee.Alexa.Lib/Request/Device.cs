using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Essenbee.Alexa.Lib.Request
{
    public class Device
    {
        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }

        [JsonProperty("supportedInterfaces")]
        public SupportedInterfaces SupportedInterfaces { get; set; }
    }
}
