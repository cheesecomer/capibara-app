namespace Capibara.Domain.Models
{
    public class Ogp: ModelBase<Ogp>
    {
        private string url;

        private string title;

        private string description;

        private string imageUrl;

        public string Url
        {
            get => this.url;
            set
            {
                this.SetProperty(ref this.url, value);
                if (this.Title.IsNullOrEmpty())
                {
                    this.Title = value.ToLower();
                }
            }
        }

        public string Title
        {
            get => this.title;
            set
            {
                this.SetProperty(ref this.title, value);
                if (this.Title.IsNullOrEmpty() && this.Url.IsPresent())
                {
                    this.Title = Url.ToLower();
                }
            }
        }

        public string Description
        {
            get => this.description;
            set => this.SetProperty(ref this.description, value);
        }

        public string ImageUrl
        {
            get => this.imageUrl;
            set => this.SetProperty(ref this.imageUrl, value);
        }

        public override Ogp Restore(Ogp other)
        {
            this.Url = other.Url;
            this.Title = other.Title;
            this.Description = other.Description;
            this.ImageUrl = other.ImageUrl;

            return this;
        }
    }
}
