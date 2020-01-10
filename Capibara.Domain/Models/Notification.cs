using System;
namespace Capibara.Domain.Models
{
    public class Notification : ModelBase<Notification>
    {
        private int id;

        private string title;

        private string message;

        private DateTimeOffset publishedAt;

        private string url;

        public int Id
        {
            get => this.id;
            set => this.SetProperty(ref this.id, value);
        }

        public string Title
        {
            get => this.title;
            set => this.SetProperty(ref this.title, value);
        }

        public string Message
        {
            get => this.message;
            set => this.SetProperty(ref this.message, value);
        }

        public DateTimeOffset PublishedAt
        {
            get => this.publishedAt;
            set => this.SetProperty(ref this.publishedAt, value);
        }

        public string Url
        {
            get => this.url;
            set => this.SetProperty(ref this.url, value);
        }

        public override Notification Restore(Notification other)
        {
            this.Id = other.Id;
            this.Title = other.Title;
            this.Message = other.Message;
            this.PublishedAt = other.PublishedAt;
            this.Url = other.Url;

            return this;
        }
    }
}
