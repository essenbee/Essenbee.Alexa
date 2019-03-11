using Essenbee.Alexa.Lib.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Essenbee.Alexa.Lib.Converters
{
    public class RequestTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var requestType = jsonObject["type"].Value<string>();

            var subType = Create(requestType);

            serializer.Populate(jsonObject.CreateReader(), subType);

            return subType;
        }

        private RequestBody Create(string requestType)
        {
            switch (requestType)
            {
                case "LaunchRequest":
                    return new LaunchRequest();
                case "IntentRequest":
                    return new IntentRequest();
                case "SessionEndRequest":
                    return new SessionEndedRequest();
                default:
                    throw new ArgumentOutOfRangeException($"Unknown request type {requestType}");
            }
        }
    }
}
