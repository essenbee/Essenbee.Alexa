using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Essenbee.Alexa.Lib;
using Essenbee.Alexa.Lib.Request;
using Essenbee.Alexa.Lib.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Essenbee.Alexa.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class AlexaController : ControllerBase
    {
        private IConfiguration _config;
        private ILogger<AlexaController> _logger;

        public AlexaController(IConfiguration config, ILogger<AlexaController> logger)
        {
            _config = config;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(AlexaResponse))]
        [ProducesResponseType(400)]
        [Route("api/alexa/devstreams")]
        public ActionResult<AlexaResponse> DevStreams ([FromBody] AlexaRequest alexaRequest )
        {
            _logger.LogInformation("Arrived here!");

            if (!alexaRequest.Session.Application.ApplicationId.Equals(_config["SkillId"]))
            {
                _logger.LogError("Bad Request - application id did not match!");

                return BadRequest();
            }

            var timeStamp = alexaRequest.RequestBody.Timestamp;
            var timeDifference = (DateTime.UtcNow - timeStamp).TotalSeconds;

            if (timeDifference <= 0 || timeDifference > 150)
            {
                _logger.LogError("Bad Request - timestamp not within accepted tolerance!");

                return BadRequest();
            }

            AlexaResponse response = null;
            var responseBuilder = new ResponseBuilder();

            switch (alexaRequest.RequestBody.Type)
            {
                case "LaunchRequest":
                    response = responseBuilder.Say("Welcome to the Dev Streams skill")
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

            var response = new ResponseBuilder()
                .Say($"You have asked about {channel} and their schedule")
                .Build();

            return response;
        }

        private AlexaResponse WhoIsLiveResponseHandler(IntentRequest intentRequest)
        {
            var response = new ResponseBuilder()
                .Say($"Codebase Alpha is streaming now")
                .Build();

            return response;
        }
    }
}