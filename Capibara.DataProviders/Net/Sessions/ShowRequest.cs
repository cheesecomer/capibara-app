using System.Net.Http;

using Newtonsoft.Json;

namespace Capibara.Net.Sessions
{
    public class ShowRequest : RequestBase<CreateResponse>
    {
        public override HttpMethod Method { get; } = HttpMethod.Get;

        public override string[] Paths { get; } = new string[] { "session" };

        public override bool NeedAuthentication { get; } = true;
    }
}
