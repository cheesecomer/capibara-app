using System.Net.Http;

namespace Capibara.Net.Users
{
    public class DestroyRequest : RequestBase
    {
        public override HttpMethod Method { get; } = HttpMethod.Delete;

        public override bool NeedAuthentication { get; } = true;

        public override string[] Paths => new string[] { "users" };
    }
}
