using System;

namespace Capibara.Domain.Models
{
    public static partial class ModelFixture
    {
        public static Ogp Ogp(
            string url = null,
            string title = null,
            string description = null,
            string imageUrl = null) => new Ogp
            {
                Url = url ?? Faker.Url.Root(),
                ImageUrl = imageUrl ?? Faker.Url.Image(),
                Title = title ?? Faker.Lorem.Sentence(),
                Description = description ?? Faker.Lorem.Paragraph()
            };
    }
}
