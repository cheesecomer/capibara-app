using System;

using Newtonsoft.Json;

namespace Capibara.Net.Sessions
{
    public class CreateResponse : Models.User
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}
