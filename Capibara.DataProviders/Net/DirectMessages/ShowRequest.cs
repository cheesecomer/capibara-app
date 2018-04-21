using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

using Capibara.Models;

namespace Capibara.Net.DirectMessages
{
    /// <summary>
    /// GET API_BASE/direct_messages/{id} へのリクエスト
    /// </summary>
    public class ShowRequest : RequestBase<ShowResponse>
    {
        private User user;

        public override HttpMethod Method => HttpMethod.Get;

        public override string[] Paths => new string[] { "direct_messages", $"{user.Id}" };

        public override bool NeedAuthentication { get; } = true;

        public override IDictionary<string, string> Query { get; }

        public override string ContentType { get; } = "application/json";

        public ShowRequest(User user, int lastId = 0)
        {
            this.user = user;

            Dictionary<string, string> query = new Dictionary<string, string>();
            if (lastId != 0)
            {
                query.Add("last_id", lastId.ToString());
            }

            this.Query = query;
        }
    }
}
