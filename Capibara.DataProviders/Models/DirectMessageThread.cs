using System;

using Newtonsoft.Json;

namespace Capibara.Models
{
    public class DirectMessageThread : ModelBase<DirectMessageThread>
    {
        private DirectMessage latestDirectMessage;

        private User user;

        [JsonProperty("latest_direct_message")]
        public DirectMessage LatestDirectMessage
        {
            get => this.latestDirectMessage;
            set => this.SetProperty(ref this.latestDirectMessage, value);
        }

        public User User
        {
            get => this.user;
            set => this.SetProperty(ref this.user, value);
        }
    }
}
