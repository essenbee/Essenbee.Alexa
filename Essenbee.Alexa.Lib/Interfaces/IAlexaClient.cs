using Essenbee.Alexa.Lib.Request;
using Essenbee.Alexa.Lib.Response;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Essenbee.Alexa.Lib.Interfaces
{
    public interface IAlexaClient
    {
        Task<string> GetUserTimezone(AlexaRequest aliexaRequest, ILogger logger);
        Task<UserAddress> GetUserAddress(AlexaRequest aliexaRequest, ILogger logger);
    }
}
