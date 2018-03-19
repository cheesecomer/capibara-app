using System.Net.Http;

using Newtonsoft.Json;

namespace Capibara.Net.Sessions
{
    public class CreateRequest : RequestBase<CreateResponse>
    {
        public override HttpMethod Method => HttpMethod.Post;

        public override string[] Paths { get; } = new string[] { "session" };

        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; }

        [JsonProperty("password", NullValueHandling = NullValueHandling.Ignore)]
        public string Password { get; }

        [JsonProperty("provider", NullValueHandling = NullValueHandling.Ignore)]
        public string Provider { get; }

        [JsonProperty("access_token", NullValueHandling = NullValueHandling.Ignore)]
        public string AccessToken { get; }

        [JsonProperty("access_token_secret", NullValueHandling = NullValueHandling.Ignore)]
        public string AccessTokenSecret { get; }

        public override string StringContent
            => JsonConvert.SerializeObject(this);

        public override string ContentType { get; } = "application/json";

        public CreateRequest(string email, string password)
        {
            this.Email = email;
            this.Password = password;
        }

        public CreateRequest(string provider, OAuth.TokenPair tokenPair)
        {
            this.Provider = provider;
            this.AccessToken = tokenPair.Token;
            this.AccessTokenSecret = tokenPair.TokenSecret;
        }
    }
}
