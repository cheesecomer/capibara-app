using System;
using System.Net.Http;

using Capibara.Models;

using Newtonsoft.Json;

namespace Capibara.Net.Users
{
    public class UpdateRequest: RequestBase<User>
    {
        private User user;
        
        public UpdateRequest(User user)
        {
            this.user = user;
        }

        public override HttpMethod Method { get; } = HttpMethod.Put;

        public override bool NeedAuthentication { get; } = true;

        public override string[] Paths
            => new string[] { "users", $"{this.user.Id}" };

        public override string StringContent
            => JsonConvert.SerializeObject(this);

        public override string ContentType { get; } = "application/json";

        [JsonProperty("nickname")]
        public string Nickname => this.user.Nickname;

        [JsonProperty("biography")]
        public string Biography => this.user.Biography;

        [JsonProperty("icon", NullValueHandling = NullValueHandling.Ignore)]
        public string IconBase64 => $"data:image/png;base64,{this.user.IconBase64}";
    }
}
