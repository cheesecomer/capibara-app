using System;

using Newtonsoft.Json;

namespace Capibara.Net.Sessions
{
    public class CreateResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("user_nickname")]
        public string Nickname { get; set; }
    }
}
