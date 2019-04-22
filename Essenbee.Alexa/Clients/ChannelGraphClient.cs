using Essenbee.Alexa.Interfaces;
using Essenbee.Alexa.Models;
using GraphQL.Client;
using GraphQL.Common.Exceptions;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Essenbee.Alexa.Clients
{
    public class ChannelGraphClient : IChannelClient
    {
        private readonly GraphQLClient _client;

        public ChannelGraphClient(GraphQLClient client)
        {
            _client = client;
        }

        public async Task<List<ChannelModel>> GetLiveChannels(string userTimeZone)
        {
            var query = new GraphQLRequest
            {
                Query = @"query getLiveChannels ($tz: String!){
                        liveChannels
                        {
                            name uri countryCode
                            schedule (timeZone: $tz)
                            { dayOfWeek localStartTime localEndTime}
                        }
                }",
                Variables = new { tz = userTimeZone}
            };

            var response = await _client.PostAsync(query);

            if (response.Errors is null)
            {
                return response.GetDataFieldAs<List<ChannelModel>>("liveChannels");
            }

            var error = response.Errors.First();
            throw new GraphQLException( new GraphQLError { Message = $"Error: {error.Message}" });
        }

        public async Task<ChannelModel> GetChannelByName(string name, string userTimeZone)
        {
            var query = new GraphQLRequest
            {
                Query = @"query getChannel ($name: String!, $tz: String!){
                        channelSoundex (name: $name)
                        {
                            name uri countryCode
                            nextStream 
                            { 
                              localStartTime (timeZone: $tz)
                              localEndTime (timeZone: $tz)
                            }
                        }
                }",
                Variables = new { name, tz = userTimeZone }
            };

            var response = await _client.PostAsync(query);

            if (response.Errors is null)
            {
                return response.GetDataFieldAs<ChannelModel>("channelSoundex");
            }

            var error = response.Errors.First();
            throw new GraphQLException(new GraphQLError { Message = $"Error: {error.Message}" });
        }
    }
}
