using System.Net.Http;

using Capibara.Models;

using Newtonsoft.Json;

namespace Capibara.Net.Blocks
{
    public class DestroyRequest: RequestBase
    {
        private int blockId;

        public override HttpMethod Method => HttpMethod.Delete;

        public override string[] Paths => new string[] { "blocks", $"{blockId}" };

        public override bool NeedAuthentication { get; } = true;

        public override string ContentType { get; } = "application/json";

        public DestroyRequest(int blockId)
        {
            this.blockId = blockId;
        }
    }
}
