using System;

namespace Capibara.Domain.Models
{
    public static partial class ModelFixture
    {
        public static Message Message(
            int? id = null,
            string content = null,
            User sender = null,
            DateTimeOffset? at = null,
            string imageUrl = null,
            string imageThumbnailUrl = null) => new Message
            {
                Id = id ?? Faker.RandomNumber.Next(),
                Content = content ?? Faker.Lorem.Sentence(),
                Sender = sender ?? ModelFixture.User(),
                At = at ?? DateTimeOffset.Now,
                ImageUrl = imageUrl ?? $"http://${Faker.Internet.DomainName()}.com/images/${Faker.RandomNumber.Next()}.png",
                ImageThumbnailUrl = imageThumbnailUrl ?? $"http://${Faker.Internet.DomainName()}.com/images/${Faker.RandomNumber.Next()}.png",
            };
    }
}
