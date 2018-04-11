using System.Net.Http;

using Capibara.Models;

using Newtonsoft.Json;

namespace Capibara.Net.Users
{
    public class UpdateRequest: RequestBase<User>
    {
        private User user;

        private bool? isAccepted;

        public UpdateRequest(User user)
        {
            this.user = user;
        }

        public UpdateRequest(bool isAccepted)
        {
            this.isAccepted = isAccepted;
        }

        public override HttpMethod Method { get; } = HttpMethod.Put;

        public override bool NeedAuthentication { get; } = true;

        public override string[] Paths
            => new string[] { "users" };

        public override string StringContent
            => JsonConvert.SerializeObject(this);

        public override string ContentType { get; } = "application/json";

        [JsonProperty("nickname", NullValueHandling = NullValueHandling.Ignore)]
        public string Nickname => this.user?.Nickname;

        [JsonProperty("biography", NullValueHandling = NullValueHandling.Ignore)]
        public string Biography => this.user?.Biography;

        [JsonProperty("accepted", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsAccepted => this.isAccepted;

        [JsonProperty("icon", NullValueHandling = NullValueHandling.Ignore)]
        public string IconBase64 => this.user?.IconBase64.IsPresent() ?? false? $"data:image/png;base64,{this.user.IconBase64}" : null;
    }
}
