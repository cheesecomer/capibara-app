using System;
using System.Net.Http;

namespace Capibara.Net.Informations
{
    public class IndexRequest : RequestBase<IndexResponse>
    {
        public override HttpMethod Method { get; } = HttpMethod.Get;

        public override bool NeedAuthentication { get; } = true;

        public override string[] Paths { get; } = new string[] { "informations" };
    }
}
