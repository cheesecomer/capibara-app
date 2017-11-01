using System.Net.Http;

using Newtonsoft.Json;

namespace Capibara.Net.Sessions
{
    public class CreateRequest : RequestBase<CreateResponse>
    {
        public override HttpMethod Method => HttpMethod.Post;

        public override string[] Paths { get; } = new string[] { "session" };

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        public override string StringContent
            => JsonConvert.SerializeObject(this);

        public override string ContentType { get; } = "application/json";
    }
}
