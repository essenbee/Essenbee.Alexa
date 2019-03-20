using Essenbee.Alexa.Lib.Interfaces;
using Essenbee.Alexa.Lib.Request;
using Essenbee.Alexa.Lib.Response;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Essenbee.Alexa.Lib.HttpClients
{
    public class AlexaClient : IAlexaClient
    {
        private readonly HttpClient _client;

        public AlexaClient(HttpClient httpClient)
        {
            _client = httpClient;
        }

        public async Task<UserAddress> GetUserAddress(AlexaRequest alexaRequest, ILogger logger)
        {
            var baseUrl = alexaRequest.Context.System.ApiEndpoint;
            var token = alexaRequest.Context.System.ApiAccessToken;
            var deviceId = alexaRequest.Context.System.Device.DeviceId;

            var url = $"{baseUrl}v1/devices/{deviceId}/settings/address";

            logger.LogInformation($"URL = {url}");
            logger.LogInformation($"Device = {deviceId}");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var result = await _client.GetAsync(url);

            logger.LogInformation($"Status = {result.StatusCode}");

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var userAddressJson = await result.Content.ReadAsStringAsync();

                var userAddress = JsonConvert.DeserializeObject<UserAddress>(userAddressJson);
                userAddress.StatusCode = result.StatusCode;

                logger.LogInformation($"User City = {userAddress.City}");
                return userAddress;
            }

            return new UserAddress(result.StatusCode);
        }

        public async Task<string> GetUserTimezone(AlexaRequest alexaRequest, ILogger logger)
        {
            var baseUrl = alexaRequest.Context.System.ApiEndpoint;
            var token = alexaRequest.Context.System.ApiAccessToken;
            var deviceId = alexaRequest.Context.System.Device.DeviceId;

            var url = $"{baseUrl}v2/devices/{deviceId}/settings/System.timeZone";

            logger.LogInformation($"URL = {url}");
            logger.LogInformation($"Device = {deviceId}");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var result = await _client.GetAsync(url);

            logger.LogInformation($"Status = {result.StatusCode}");

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var timezone = await result.Content.ReadAsStringAsync();

                logger.LogInformation($"TZ = {timezone}");
                return timezone;
            }

            return String.Empty;
        }
    }
}
