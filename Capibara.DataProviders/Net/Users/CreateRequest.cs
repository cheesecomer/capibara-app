using System.Net.Http;

using Newtonsoft.Json;

using CreateResponse = Capibara.Net.Sessions.CreateResponse;

namespace Capibara.Net.Users
{
    public class CreateRequest : RequestBase<CreateResponse>
    {
        public override HttpMethod Method => HttpMethod.Post;

        public override string[] Paths { get; } = new string[] { "users" };

        [JsonProperty("nickname")]
        public string Nickname { get; set; }

        public override string StringContent
            => JsonConvert.SerializeObject(this);

        public override string ContentType { get; } = "application/json";
    }
}
