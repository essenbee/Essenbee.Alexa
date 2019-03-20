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
            if (_response.Response is null)
            {
                _response.Response = new ResponseBody();
            }

            _response.Response.ShouldEndSession = true;
            _response.Response.OutputSpeech = new TextOutput(text);
            return this;
        }

        public ResponseBuilder SayWithSsml(string ssml)
        {
            if (_response.Response is null)
            {
                _response.Response = new ResponseBody();
            }

            _response.Response.ShouldEndSession = true;
            _response.Response.OutputSpeech = new SsmlSpeech(ssml);
            return this;
        }

        public ResponseBuilder WriteSimpleCard(string title, string text)
        {
            if (_response.Response is null)
            {
                _response.Response = new ResponseBody();
            }

            _response.Response.ShouldEndSession = true;
            _response.Response.Card = new SimpleCard(title, text);
            return this;
        }

        public ResponseBuilder WriteStandardCard(string title, string text)
        {
            if (_response.Response is null)
            {
                _response.Response = new ResponseBody();
            }

            _response.Response.ShouldEndSession = true;
            _response.Response.Card = new StandardCard(title, text);
            return this;
        }

        public ResponseBuilder WriteStandardCard(string title, string text, 
            string smallImageUrl, string largeImageUrl)
        {
            if (_response.Response is null)
            {
                _response.Response = new ResponseBody();
            }

            _response.Response.ShouldEndSession = true;
            _response.Response.Card = new StandardCard(title, text, smallImageUrl, largeImageUrl);
            return this;
        }

        public ResponseBuilder WriteAskForPermissionsCard(string[] permissions)
        {
            if (_response.Response is null)
            {
                _response.Response = new ResponseBody();
            }

            _response.Response.ShouldEndSession = true;
            _response.Response.Card = new AskForPermissionsConsent(permissions);
            return this;
        }

        public AlexaResponse Build()
        {
            return _response;
        }
    }
}
