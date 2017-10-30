using System;
using System.Net.Http;

namespace Capibara.Net.Rooms
{
    /// <summary>
    /// GET API_BASE/rooms へのリクエスト
    /// </summary>
    public class IndexRequest : RequestBase<IndexResponse>
    {
        public override HttpMethod Method { get; } = HttpMethod.Get;

        public override bool NeedAuthentication { get; } = true;

        public override string[] Paths { get; } = new string[] { "rooms" };
    }
}
