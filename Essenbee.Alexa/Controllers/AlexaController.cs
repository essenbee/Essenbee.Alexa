using System.Linq;
using System.Threading.Tasks;
using Essenbee.Alexa.Lib;
using Essenbee.Alexa.Lib.Interfaces;
using Essenbee.Alexa.Lib.Request;
using Essenbee.Alexa.Lib.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
        private UserAddress _userAddress;

        public AlexaController(IConfiguration config, ILogger<AlexaController> logger, IAlexaClient client)
        {
            _config = config;
            _logger = logger;
            _client = client;
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(AlexaResponse))]
        [ProducesResponseType(400)]
        [Route("api/alexa/devstreams")]
        public async Task<ActionResult<AlexaResponse>> DevStreams ([FromBody] AlexaRequest alexaRequest )
        {
            _logger.LogInformation("Arrived here!");

            if (!AlexaRequest.ShouldProcessRequest(_config["SkillId"], alexaRequest))
            {
                _logger.LogError("Bad Request - application id did not match or timestamp tolerance exceeded!");

                return BadRequest();
            }

            AlexaResponse response = null;
            var responseBuilder = new ResponseBuilder();

            _userTimeZone = await _client.GetUserTimezone(alexaRequest, _logger);
            _userAddress = await _client.GetUserAddress(alexaRequest, _logger);

            if (_userAddress.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                response = responseBuilder.Say("You have not given permission to read your address details. Some aspects of this skill may not function optimally.")
                    .WriteAskForPermissionsCard(new string[] { "read::alexa:device:all:address" })
                    .Build();

                return response;
            }

            switch (alexaRequest.RequestBody.Type)
            {
                case "LaunchRequest":
                    var ssml = @"<speak>Welcome to the Dev Streams skill</speak>";
                    response = responseBuilder.SayWithSsml(ssml)
                        .Build();
                    break;
                case "IntentRequest":
                    response = IntentRequestHandler(alexaRequest);
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
            return null;
        }

        private AlexaResponse IntentRequestHandler(AlexaRequest alexaRequest)
        {
            var intentRequest = alexaRequest.RequestBody as IntentRequest;

            //var response = new ResponseBuilder()
            //    .Say($"I received an intent request {intentRequest.Intent.Name}")
            //    .Build();


            AlexaResponse response = null;

            if (intentRequest != null)
            {
                switch (intentRequest.Intent.Name)
                {
                    case "whenNextIntent":
                        response = WhenNextResponseHandler(intentRequest);
                        break;
                    case "whoIsLiveIntent":
                        response = WhoIsLiveResponseHandler(intentRequest);
                        break;
                    case "AMAZON.StopIntent":
                    case "AMAZON.CancelIntent":
                        response = CancelOrStopResponseHandler(intentRequest);
                        break;
                    case "AMAZON.HelpIntent":
                        response = HelpIntentResponseHandler(intentRequest);
                        break;
                }
            }

            return response;
        }

        private AlexaResponse HelpIntentResponseHandler(IntentRequest intentRequest)
        {
            var response = new ResponseBuilder()
                .Say("To use this skill, ask me about the schedule of your favourite stream. " +
                "You can also say Alexa stop to exit the skill")
                .Build(); ;
            return response;
        }

        private AlexaResponse CancelOrStopResponseHandler(IntentRequest intentRequest)
        {
            var response = new ResponseBuilder()
                .Say("Thanks for using the Dev Streams skill")
                .Build();

            return response;
        }

        private AlexaResponse WhenNextResponseHandler(IntentRequest intentRequest)
        {
            var channel = "some streamer";

            if (intentRequest.Intent.Slots.Any())
            {
                channel = intentRequest.Intent.Slots["channel"].Value;
            }

            var city = _userAddress.StatusCode == System.Net.HttpStatusCode.OK
                ? $" in {_userAddress.City}"
                : string.Empty;

            var response = new ResponseBuilder()
                .Say($"You have asked about {channel} and their schedule in your timezone {_userTimeZone}{city}")
                .Build();

            return response;
        }

        private AlexaResponse WhoIsLiveResponseHandler(IntentRequest intentRequest)
        {
            var response = new ResponseBuilder()
                .Say($"Codebase Alpha is streaming now")
                .WriteSimpleCard("Streaming Now!", "Codebase Alpha")
                .Build();

            var jsonResponse = JsonConvert.SerializeObject(response);
            _logger.LogInformation($"{jsonResponse}");

            return response;
        }
    }
}