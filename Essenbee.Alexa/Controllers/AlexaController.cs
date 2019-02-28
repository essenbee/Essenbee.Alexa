using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Essenbee.Alexa.Lib.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Essenbee.Alexa.Controllers
{
    [ApiController]
    public class AlexaController : ControllerBase
    {
        [HttpPost]
        [Route("api/alexa/devstreams")]
        public void DevStreams ([FromBody] AlexaRequest alexaRequest )
        {
            // ToDo: ensure that request is meant for us

            var req = alexaRequest;

        }
    }
}