using Essenbee.Alexa.Lib;
using Essenbee.Alexa.Lib.Request;
using Essenbee.Alexa.Lib.Response;
using Essenbee.Alexa.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Essenbee.Alexa.Alexa
{
    public static class Responses
    {
        public static AlexaResponse GetNextStreamResponse(ChannelModel channel)
        {
            AlexaResponse response = null;

            if (channel != null && !string.IsNullOrWhiteSpace(channel.Name))
            {
                var name = channel.Name;
                var nextStream = channel.NextStream;
                var nextStreamTimeFormatted = "currently has no future streams set up in the Dev Streams database";

                if (nextStream != null)
                {
                    var zonedDateTime = nextStream.LocalStartTime;

                    if (nextStream.UtcStartTime.Date == DateTime.UtcNow.Date)
                    {
                        nextStreamTimeFormatted = string.Format("will be streaming next today at {0:h:mm tt}", zonedDateTime);
                    }
                    else if (nextStream.UtcStartTime.Date == DateTime.UtcNow.Date.AddDays(1))
                    {
                        nextStreamTimeFormatted = string.Format("will be streaming next tomorrow at {0:h:mm tt}", zonedDateTime);
                    }
                    else
                    {
                        nextStreamTimeFormatted = string.Format("will be streaming next on {0:dddd, MMMM dd} at {0:h:mm tt}", zonedDateTime, zonedDateTime);
                    }
                }

                response = new ResponseBuilder()
                    .Say($"{name} {nextStreamTimeFormatted}")
                    .WriteSimpleCard($"{name}", $"{name} {nextStreamTimeFormatted}")
                    .Build();
            }
            else
            {
                response = new ResponseBuilder()
                    .Say($"Sorry, I could not find {channel} in my database of live coding streamers")
                    .WriteSimpleCard("Not Found", $"{channel} is not in the DevStreams database")
                    .Build();
            }

            return response;
        }

        public static AlexaResponse GetLiveNowResponse(List<ChannelModel> liveChannels)
        {
            var response = new ResponseBuilder()
                .Say("None of the streamers in my database are currently broadcasting")
                .WriteSimpleCard("Streaming Now!", "None")
                .Build();

            if (liveChannels != null && liveChannels.Count > 0)
            {
                var channelNames = liveChannels.Select(x => x.Name).ToList();
                var firstFew = string.Join(", ", channelNames.Take(3));

                var howMany = channelNames.Count == 1
                    ? $"{channelNames.First()} is broadcasting now."
                    : $"{channelNames.Count} streamers are broadcasting now:";

                var streamers = string.Empty;

                if (channelNames.Count == 2)
                {
                    streamers = $"{channelNames[0]} and {channelNames[1]}";
                }

                if (liveChannels.Count == 3)
                {
                    streamers = $"{channelNames[0]}, {channelNames[1]} and {channelNames[2]}";
                }

                if (liveChannels.Count > 3)
                {
                    streamers = $"Here are the first three: {firstFew}";
                }

                response = new ResponseBuilder()
                    .Say($"{howMany} {streamers}")
                    .WriteSimpleCard("Streaming Now!", $"{firstFew}")
                    .Build();
            }

            return response;
        }

        public static AlexaResponse GiveLaunchResponse()
        {
            var text = "Welcome to the Dev Streams skill. Ask me who is streaming now " +
                        "or when your favourite channels are streaming next";
            var reprompt = "Ask me who is streaming now " +
                "or when your favourite channels are streaming next";

            return new ResponseBuilder()
                .Ask(text, reprompt)
                .Build();
        }

        public static AlexaResponse GiveHelpResponse()
        {
            var text = "To use this skill, ask me about when your favourite channel is streaming next; or who is broadcasting now. " +
                       "You can also say Alexa Stop to exit the skill";
            var reprompt = "Ask me who is streaming now " +
                        "or when your favourite channels are streaming next";

            var response = new ResponseBuilder()
                .Ask(text, reprompt)
                .Build();
            return response;
        }

        public static AlexaResponse GiveStopResponse()
        {
            var response = new ResponseBuilder()
                    .Say("Thanks for using the Dev Streams skill")
                    .Build();
            return response;
        }

        public static AlexaResponse GiveFallbackResponse()
        {
            // Assume we have a WhenNext intent
            var channelSlot = new Slot
            {
                Name = "channel",
                Value = string.Empty,
                ConfirmationStatus = "None"
            };

            var whenNextIntent = new Intent
            {
                Name = "whenNextIntent",
                ConfirmationStatus = "None",
                Slots = new Dictionary<string, Slot>
                {
                    { "channel", channelSlot }
                }
            };

            return new ResponseBuilder()
                .DialogDelegate(whenNextIntent)
                .Build();
        }
    }
}
