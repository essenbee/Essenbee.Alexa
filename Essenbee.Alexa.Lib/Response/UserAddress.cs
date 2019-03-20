using Newtonsoft.Json;
using System.Net;

namespace Essenbee.Alexa.Lib.Response
{
    public class UserAddress
    {
        [JsonProperty("stateOrRegion")]
        public string StateOrRegion { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public UserAddress()
        {
        }

        public UserAddress(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }
    }
}
