using System;
using System.Net.Http;

namespace Capibara.Net.DirectMessages
{
    /// <summary>
    /// GET API_BASE/direct_messages へのリクエスト
    /// </summary>
    public class IndexRequest : RequestBase<IndexResponse>
    {
        public override HttpMethod Method { get; } = HttpMethod.Get;

        public override bool NeedAuthentication { get; } = true;

        public override string[] Paths { get; } = new string[] { "direct_messages" };

        public override string ContentType { get; } = "application/json";
    }
}
