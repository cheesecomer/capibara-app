using System.Net.Http;

using Capibara.Models;

using Newtonsoft.Json;

namespace Capibara.Net.Blocks
{
    public class DestroyRequest: RequestBase
    {
        private Block block;

        public override HttpMethod Method => HttpMethod.Delete;

        public override string[] Paths => new string[] { "blocks", $"{block.Id}" };

        public override bool NeedAuthentication { get; } = true;

        public override string ContentType { get; } = "application/json";

        public DestroyRequest(Block block)
        {
            this.block = block;
        }
    }
}
