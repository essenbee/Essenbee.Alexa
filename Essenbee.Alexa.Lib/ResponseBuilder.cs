using Essenbee.Alexa.Lib.Request;
using Essenbee.Alexa.Lib.Response;

namespace Essenbee.Alexa.Lib
{
    public class ResponseBuilder
    {
        private AlexaResponse _response;

        public ResponseBuilder()
        {
            _response = new AlexaResponse();
        }

        public ResponseBuilder Say(string speech)
        {
            if (_response.Response is null)
            {
                _response.Response = new ResponseBody();
            }

            _response.Response.ShouldEndSession = true;
            _response.Response.OutputSpeech = new TextOutput(speech);
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

        public ResponseBuilder Ask(string speech)
        {
            if (_response.Response is null)
            {
                _response.Response = new ResponseBody();
            }

            _response.Response.ShouldEndSession = false;
            _response.Response.OutputSpeech = new TextOutput(speech);
            return this;
        }

        public ResponseBuilder Ask(string speech, string repromptSpeech)
        {
            if (_response.Response is null)
            {
                _response.Response = new ResponseBody();
            }

            _response.Response.ShouldEndSession = false;
            _response.Response.OutputSpeech = new TextOutput(speech);
            _response.Response.Reprompt = new Reprompt
            {
                OutputSpeech = new TextOutput(repromptSpeech)
            };

            return this;
        }

        public ResponseBuilder DialogDelegate(Intent updatedIntent = null)
        {
            if (_response.Response is null)
            {
                _response.Response = new ResponseBody();
            }

            _response.Response.ShouldEndSession = false;
            _response.Response.Directives.Add(new DialogDelegate
            {
                UpdatedIntent = updatedIntent
            });

            return this;
        }

        public ResponseBuilder DialogConfirmSlot(string speech, string slot, Intent updatedIntent = null)
        {
            if (_response.Response is null)
            {
                _response.Response = new ResponseBody();
            }

            _response.Response.ShouldEndSession = false;
            _response.Response.OutputSpeech = new TextOutput(speech);
            _response.Response.Directives.Add(new DialogConfirmSlot(slot)
            {
                UpdatedIntent = updatedIntent
            });

            return this;
        }

        public ResponseBuilder DialogAskForSlot(string speech, string slot, Intent updatedIntent = null)
        {
            if (_response.Response is null)
            {
                _response.Response = new ResponseBody();
            }

            _response.Response.ShouldEndSession = false;
            _response.Response.OutputSpeech = new TextOutput(speech);
            _response.Response.Directives.Add(new DialogElicitSlot(slot)
            {
                UpdatedIntent = updatedIntent
            });

            return this;
        }

        public AlexaResponse Build()
        {
            return _response;
        }
    }
}
