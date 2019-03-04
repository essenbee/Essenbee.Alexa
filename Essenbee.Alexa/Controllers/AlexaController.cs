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
            var responseBuilder = new SkillResponse();

            switch (alexaRequest.RequestBody.Type)
            {
                case "LaunchRequest":
                    response = responseBuilder.AddTextSpeech("Welcome to the Dev Streams skill")
                        .Build();
                    break;
                default:
                    break;
            }

            return response;
        }
    }
}