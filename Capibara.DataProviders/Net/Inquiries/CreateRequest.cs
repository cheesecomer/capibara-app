using System.Net.Http;

using Capibara.Models;

using Newtonsoft.Json;

namespace Capibara.Net.Inquiries
{
    public class CreateRequest : RequestBase
    {
        public override HttpMethod Method => HttpMethod.Post;

        public override string[] Paths { get; } = new string[] { "inquiries" };

        public override bool NeedAuthentication { get; } = true;

        [JsonProperty("email")]
        public string Email { get; }

        [JsonProperty("content")]
        public string Content { get; }

        public override string StringContent
            => JsonConvert.SerializeObject(this);

        public override string ContentType { get; } = "application/json";

        public CreateRequest(string email, string content)
        {
            this.Email = email;
            this.Content = content;
        }
    }
}
