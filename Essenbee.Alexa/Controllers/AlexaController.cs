using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Essenbee.Alexa.Lib;
using Essenbee.Alexa.Lib.Request;
using Essenbee.Alexa.Lib.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Essenbee.Alexa.Controllers
{
    [ApiController]
    public class AlexaController : ControllerBase
    {
        [HttpPost]
        [Route("api/alexa/devstreams")]
        public AlexaResponse DevStreams ([FromBody] AlexaRequest alexaRequest )
        {
            // ToDo: ensure that request is meant for us

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
                default:
                    break;
            }

            return response;
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
                    case "devstreams":
                        response = DevStreamsResponseHandler(intentRequest);
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

        private AlexaResponse DevStreamsResponseHandler(IntentRequest intentRequest)
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
    }
}