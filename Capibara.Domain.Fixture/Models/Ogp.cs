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
                Url = url ?? $"http://${Faker.Internet.DomainName()}.com/",
                ImageUrl = imageUrl ?? $"http://${Faker.Internet.DomainName()}.com/images/${Faker.RandomNumber.Next()}.png",
                Title = title ?? Faker.Lorem.Sentence(),
                Description = description ?? Faker.Lorem.Paragraph()
            };
    }
}
