using System.Threading.Tasks;
using Essenbee.Alexa.Alexa;
using Essenbee.Alexa.Lib;
using Essenbee.Alexa.Lib.Interfaces;
using Essenbee.Alexa.Lib.Request;
using Essenbee.Alexa.Lib.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Essenbee.Alexa.Interfaces;

namespace Essenbee.Alexa.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class AlexaController : ControllerBase
    {
        private IConfiguration _config;
        private ILogger<AlexaController> _logger;
        private readonly IAlexaClient _client;
        private string _userTimeZone = string.Empty;
        private IChannelClient _channelClient;

        public AlexaController(IConfiguration config, ILogger<AlexaController> logger, 
            IAlexaClient client, IChannelClient channelClient)
        {
            _config = config;
            _logger = logger;
            _client = client;
            _channelClient = channelClient;
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(AlexaResponse))]
        [ProducesResponseType(400)]
        [Route("api/alexa/devstreams")]
        public async Task<ActionResult<AlexaResponse>> DevStreams([FromBody] AlexaRequest alexaRequest)
        {
            if (!AlexaRequest.ShouldProcessRequest(_config["SkillId"], alexaRequest))
            {
                _logger.LogError("Bad Request - application id did not match or timestamp tolerance exceeded!");

                return BadRequest();
            }

            AlexaResponse response = null;

            _userTimeZone = await _client.GetUserTimezone(alexaRequest, _logger);
            _userTimeZone = _userTimeZone.Replace("\"", string.Empty);

            switch (alexaRequest.RequestBody.Type)
            {
                case "LaunchRequest":
                    response = Responses.GiveLaunchResponse();
                    break;
                case "IntentRequest":
                    response = await IntentRequestHandler(alexaRequest);
                    break;
                case "SessionEndedRequest":
                    response = SessionEndedRequestHandler(alexaRequest);
                    break;
                default:
                    break;
            }

            return response;
        }

        private AlexaResponse SessionEndedRequestHandler(AlexaRequest alexaRequest)
        {
            var sessionEndedRequest = alexaRequest.RequestBody as SessionEndedRequest;
            if (sessionEndedRequest.Error != null)
            {
                var error = sessionEndedRequest.Error;
                _logger.LogError($"{error.ErrorType} - {error.ErrorMessage}");
            }

            if (sessionEndedRequest.Reason == Reason.UserInitiated)
            {
                var response = new ResponseBuilder()
                .Say(string.Empty)
                .Build();
                return response;
            }

            return null;
        }

        private async Task<AlexaResponse> IntentRequestHandler(AlexaRequest alexaRequest)
        {
            AlexaResponse response = null;

            if (alexaRequest.RequestBody is IntentRequest intentRequest)
            {
                switch (intentRequest.Intent.Name)
                {
                    case "whenNextIntent":
                        response = await WhenNextResponseHandler(intentRequest);
                        break;
                    case "whoIsLiveIntent":
                        response = await WhoIsLiveResponseHandler(intentRequest);
                        break;
                    case "AMAZON.StopIntent":
                    case "AMAZON.CancelIntent":
                        response = CancelOrStopResponseHandler(intentRequest);
                        break;
                    case "AMAZON.HelpIntent":
                        response = HelpIntentResponseHandler(intentRequest);
                        break;
                    case "AMAZON.FallbackIntent":
                        response = FallbackIntentResponseHandler(intentRequest);
                        break;
                }
            }

            return response;
        }

        private AlexaResponse FallbackIntentResponseHandler(IntentRequest intentRequest)
        {
            return Responses.GiveFallbackResponse();
        }

        private AlexaResponse HelpIntentResponseHandler(IntentRequest intentRequest)
        {
            return Responses.GiveHelpResponse();
        }

        private AlexaResponse CancelOrStopResponseHandler(IntentRequest intentRequest)
        {
            return Responses.GiveStopResponse();
        }
        private async Task<AlexaResponse> WhenNextResponseHandler(IntentRequest intentRequest)
        {
            var channel = intentRequest.Intent.Slots["channel"].Value;
            var standardisedChannel = channel
                .Replace(" ", string.Empty)
                .Replace(".", string.Empty);

            var matchingChannel = await _channelClient.GetChannelByName(standardisedChannel, _userTimeZone);
            var response = Responses.GetNextStreamResponse(matchingChannel);

            return response;
        }
        private async Task<AlexaResponse> WhoIsLiveResponseHandler(IntentRequest intentRequest)
        {
            var liveChannels = await _channelClient.GetLiveChannels(_userTimeZone);
            var response = Responses.GetLiveNowResponse(liveChannels);

            return response;
        }
    }
}