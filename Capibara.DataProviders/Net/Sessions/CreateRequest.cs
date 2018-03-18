using System.Net.Http;

using Newtonsoft.Json;

namespace Capibara.Net.Sessions
{
    public class CreateRequest : RequestBase<CreateResponse>
    {
        public override HttpMethod Method => HttpMethod.Post;

        public override string[] Paths { get; } = new string[] { "session" };

        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }

        [JsonProperty("password", NullValueHandling = NullValueHandling.Ignore)]
        public string Password { get; set; }

        [JsonProperty("provider", NullValueHandling = NullValueHandling.Ignore)]
        public string Provider { get; set; }

        [JsonProperty("access_token", NullValueHandling = NullValueHandling.Ignore)]
        public string AccessToken { get; set; }

        [JsonProperty("access_token_secret", NullValueHandling = NullValueHandling.Ignore)]
        public string AccessTokenSecret { get; set; }

        public override string StringContent
            => JsonConvert.SerializeObject(this);

        public override string ContentType { get; } = "application/json";
    }
}
