using System.Net.Http;

using Capibara.Models;

using Newtonsoft.Json;

namespace Capibara.Net.Follows
{
    public class DestroyRequest : RequestBase
    {
        private int followId;

        public override HttpMethod Method => HttpMethod.Delete;

        public override string[] Paths => new string[] { "follows", $"{this.followId}" };

        public override bool NeedAuthentication { get; } = true;

        public override string StringContent
            => JsonConvert.SerializeObject(this);

        public override string ContentType { get; } = "application/json";

        public DestroyRequest(int followId)
        {
            this.followId = followId;
        }
    }
}
