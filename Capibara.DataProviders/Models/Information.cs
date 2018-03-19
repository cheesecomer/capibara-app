using System;

using Newtonsoft.Json;
namespace Capibara.Models
{
    public class Information : ModelBase<Information>
    {
        private int id;

        private string title;

        private string message;

        private DateTimeOffset publishedAt;

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

        [JsonProperty("published_at")]
        public DateTimeOffset PublishedAt
        {
            get => this.publishedAt;
            set => this.SetProperty(ref this.publishedAt, value);
        }

        public override void Restore(Information model)
        {
            base.Restore(model);

            this.Id = model.Id;
            this.Title = model.Title;
            this.Message = model.Message;
            this.PublishedAt = model.PublishedAt;
        }
    }
}
