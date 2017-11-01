using System;
using System.Net.Http;

using Capibara.Models;

namespace Capibara.Net.Users
{
    public class ShowRequest: RequestBase<User>
    {
        private User user;

        public override HttpMethod Method { get; } = HttpMethod.Get;

        public override bool NeedAuthentication { get; } = true;

        public override string[] Paths
            => new string[] { "users", $"{this.user.Id}" };

        public ShowRequest(User user)
        {
            this.user = user;
        }
    }
}
