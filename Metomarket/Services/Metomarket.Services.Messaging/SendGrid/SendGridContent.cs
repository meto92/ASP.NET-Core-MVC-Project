﻿using Newtonsoft.Json;

namespace Metomarket.Services.Messaging.SendGrid
{
    public class SendGridContent
    {
        public SendGridContent()
        {
        }

        public SendGridContent(string type, string content)
        {
            this.Type = type;
            this.Value = content;
        }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}