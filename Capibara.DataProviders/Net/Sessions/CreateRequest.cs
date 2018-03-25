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

        public override string StringContent
            => JsonConvert.SerializeObject(this);

        public override string ContentType { get; } = "application/json";

        public CreateRequest(string email, string password)
        {
            this.Email = email;
            this.Password = password;
        }
    }
}
