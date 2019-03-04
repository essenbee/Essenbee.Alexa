using Essenbee.Alexa.Lib.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Essenbee.Alexa.Lib
{
    public class SkillResponse
    {
        private AlexaResponse _response;

        public SkillResponse()
        {
            _response = new AlexaResponse();
        }

        public SkillResponse AddTextSpeech(string text)
        {
            _response.Response = new ResponseBody();
            _response.Response.OutputSpeech = new TextOutput(text);
            return this;
        }

        public AlexaResponse Build()
        {
            return _response;
        }
    }
}
