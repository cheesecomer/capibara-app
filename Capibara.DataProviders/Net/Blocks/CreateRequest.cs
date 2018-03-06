using System.Net.Http;

using Capibara.Models;

using Newtonsoft.Json;

namespace Capibara.Net.Blocks
{
    public class CreateRequest : RequestBase<IndexResponse>
    {
        private User target;

        public override HttpMethod Method => HttpMethod.Post;

        public override string[] Paths { get; } = new string[] { "blocks" };

        [JsonProperty("target_id")]
        public int TargetId => this.target.Id;

        public override string StringContent
            => JsonConvert.SerializeObject(this);

        public override string ContentType { get; } = "application/json";

        public CreateRequest(User target)
        {
            this.target = target;
        }
    }
}
