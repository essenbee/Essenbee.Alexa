using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Essenbee.Alexa.Lib.Request
{
    public class Resolutions
    {
        [JsonProperty("resolutionsPerAuthority")]
        public ResolutionsPerAuthority[] ResolutionsPerAuthority { get; set; }
    }
}
