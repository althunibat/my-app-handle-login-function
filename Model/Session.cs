﻿using System.Text.Json.Serialization;

namespace Godwit.HandleLoginAction.Model
{
    public class Session
    {
        [JsonPropertyName("x-hasura-user-id")]
        public string UserId { get; set; }

        [JsonPropertyName("x-hasura-role")]
        public string Role { get; set; }
    }
}