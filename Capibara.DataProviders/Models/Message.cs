using System;

using Newtonsoft.Json;
namespace Capibara.Models
{
    public class Message : ModelBase<Message>
    {
        private int id;

        private string content;

        private DateTimeOffset at;

        private User sender;

        private string imageUrl;

        public int Id
        {
            get => this.id;
            set => this.SetProperty(ref this.id, value);
        }

        public string Content
        {
            get => this.content;
            set => this.SetProperty(ref this.content, value);
        }

        public DateTimeOffset At
        {
            get => this.at;
            set => this.SetProperty(ref this.at, value);
        }

        public User Sender
        {
            get => this.sender;
            set => this.SetProperty(ref this.sender, value);
        }

        [JsonProperty("image")]
        public string ImageUrl
        {
            get => this.imageUrl;
            set => this.SetProperty(ref this.imageUrl, value);
        }

        public bool IsOwn => this.IsolatedStorage.UserId == this.Sender?.Id;
    }
}
