﻿using System;
using System.Linq;

namespace Essenbee.Alexa.Models
{
    public class TwitchModel
    {
        public string ChannelId { get; set; }
        public string TwitchId { get; set; }
        public string TwitchName { get; set; }
        public bool IsAffiliate { get; set; }
        public bool IsPartner { get; set; }
    }
}
