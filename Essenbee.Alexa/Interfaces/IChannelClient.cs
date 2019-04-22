using Essenbee.Alexa.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Essenbee.Alexa.Interfaces
{
    public interface IChannelClient
    {
        Task<ChannelModel> GetChannelByName(string channelName, string userTimeZone);
        Task<List<ChannelModel>> GetLiveChannels(string userTimeZone);
    }
}
