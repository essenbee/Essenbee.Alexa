using Essenbee.Alexa.Lib.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Essenbee.Alexa.Lib
{
    public class ResponseBuilder
    {
        private AlexaResponse _response;

        public ResponseBuilder()
        {
            _response = new AlexaResponse();
        }

        public ResponseBuilder Say(string text)
        {
            _response.Response = new ResponseBody();
            _response.Response.ShouldEndSession = true;
            _response.Response.OutputSpeech = new TextOutput(text);
            return this;
        }

        public AlexaResponse Build()
        {
            return _response;
        }
    }
}
