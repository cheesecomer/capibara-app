using System;
namespace Capibara.Domain.Models
{
    public class Message : ModelBase<Message>
    {
        private int id;

        private string content;

        private DateTimeOffset at;

        private User sender;

        private string imageUrl;

        private string imageThumbnailUrl;

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

        public string ImageUrl
        {
            get => this.imageUrl;
            set => this.SetProperty(ref this.imageUrl, value);
        }

        public string ImageThumbnailUrl
        {
            get => this.imageThumbnailUrl;
            set => this.SetProperty(ref this.imageThumbnailUrl, value);
        }

        public override Message Restore(Message other)
        {
            this.At = other.At;
            this.Id = other.Id;
            this.Sender = other.Sender;
            this.Content = other.content;
            this.ImageUrl = other.ImageUrl;
            this.ImageThumbnailUrl = other.ImageThumbnailUrl;

            return this;
        }
    }
}
