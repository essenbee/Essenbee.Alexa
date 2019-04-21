using Essenbee.Alexa.Lib;
using Essenbee.Alexa.Lib.Request;
using Essenbee.Alexa.Lib.Response;
using System.Collections.Generic;
using System.Linq;

namespace Essenbee.Alexa.Alexa
{
    public static class Responses
    {
        //public static async Task<AlexaResponse> GetNextStreamResponse(string userTimeZone, string channel)
        //{
        //    AlexaResponse response = null;

        //    if (dbChannel != null && !string.IsNullOrWhiteSpace(dbChannel.Name))
        //    {
        //        var name = dbChannel.Name;
        //        var id = dbChannel.Id;
        //        var sessions = new List<StreamSession>();

        //        string query = $"SELECT * FROM StreamSessions WHERE ChannelId = @id AND UtcStartTime > GETUTCDATE() ORDER BY UtcStartTime";
        //        using (System.Data.IDbConnection connection = new SqlConnection(dbSettings.DefaultConnection))
        //        {
        //            using (var multi = await connection.QueryMultipleAsync(query, new { id }))
        //            {
        //                sessions = (await multi.ReadAsync<StreamSession>()).ToList();
        //            }
        //        }

        //        var nextStream = new StreamSession();
        //        var zonedDateTime = DateTime.MinValue;
        //        var nextStreamTimeFormatted = "currently has no future streams set up in the Dev Streams database";

        //        if (sessions.Count > 0)
        //        {
        //            nextStream = sessions.FirstOrDefault();

        //            if (nextStream != null)
        //            {
        //                DateTimeZone zone = DateTimeZoneProviders.Tzdb[userTimeZone];
        //                zonedDateTime = nextStream.UtcStartTime.InZone(zone).ToDateTimeUnspecified();
        //                nextStreamTimeFormatted = string.Format("will be streaming next on {0:dddd, MMMM dd} at {0:h:mm tt}", zonedDateTime, zonedDateTime);
        //            }
        //        }

        //        response = new ResponseBuilder()
        //            .Say($"{name} {nextStreamTimeFormatted}")
        //            .WriteSimpleCard($"{name}", $"{name} {nextStreamTimeFormatted}")
        //            .Build();
        //    }
        //    else
        //    {
        //        response = new ResponseBuilder()
        //            .Say($"Sorry, I could not find {channel} in my database of live coding streamers")
        //            .WriteSimpleCard("Not Found", $"{channel} is not in the DevStreams database")
        //            .Build();
        //    }

        //    return response;
        //}

        public static AlexaResponse GetLiveNowResponse(List<string> liveChannels)
        {
            var response = new ResponseBuilder()
                .Say("None of the streamers in my database are currently broadcasting")
                .WriteSimpleCard("Streaming Now!", "None")
                .Build();

            if (liveChannels != null && liveChannels.Count > 0)
            {
                var firstFew = string.Join(", ", liveChannels.Take(3));

                var howMany = liveChannels.Count == 1
                    ? $"{liveChannels.First()} is broadcasting now."
                    : $"{liveChannels.Count} streamers are broadcasting now:";

                var streamers = string.Empty;

                if (liveChannels.Count == 2)
                {
                    streamers = $"{liveChannels[0]} and {liveChannels[1]}";
                }

                if (liveChannels.Count == 3)
                {
                    streamers = $"{liveChannels[0]}, {liveChannels[1]} and {liveChannels[2]}";
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
