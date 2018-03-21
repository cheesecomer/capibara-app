﻿using System.Net.Http;

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
            => new string[] { "users" };

        public override string StringContent
            => JsonConvert.SerializeObject(this);

        public override string ContentType { get; } = "application/json";

        [JsonProperty("nickname")]
        public string Nickname => this.user.Nickname;

        [JsonProperty("biography")]
        public string Biography => this.user.Biography;

        [JsonProperty("accepted")]
        public bool IsAccepted => this.user.IsAccepted;

        [JsonProperty("icon", NullValueHandling = NullValueHandling.Ignore)]
        public string IconBase64 => this.user.IconBase64.IsPresent() ? $"data:image/png;base64,{this.user.IconBase64}" : null;
    }
}
