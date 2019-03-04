using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Essenbee.Alexa.Lib.Interfaces
{
    public interface IResponseType
    {
        [JsonRequired]
        [JsonProperty("type")]
        string Type { get; }
    }
}
