using Essenbee.Alexa.Lib.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Essenbee.Alexa.Lib.Request
{
    public class AlexaRequest
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("session")]
        public Session Session { get; set; }

        [JsonProperty("context")]
        public Context Context { get; set; }

        [JsonProperty("request"), JsonConverter(converterType: typeof(RequestTypeConverter))]
        public RequestBody RequestBody { get; set; }

        public static AlexaRequest FromJson(string json) => JsonConvert.DeserializeObject<AlexaRequest>(json, Converter.Settings);

        public static bool ShouldProcessRequest(string applicationId, AlexaRequest alexaRequest)
        {
            if (!alexaRequest.Session.Application.ApplicationId.Equals(applicationId))
            {
                return false;
            }

            var timeStamp = alexaRequest.RequestBody.Timestamp;
            var timeDifference = (DateTime.UtcNow - timeStamp).TotalSeconds;

            if (timeDifference <= 0 || timeDifference > 150)
            {
                return false;
            }

            return true;
        }
    }
}

internal static class Converter
{
    public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
    };
}