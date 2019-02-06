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
                ImageUrl = imageUrl ?? Faker.Url.Image(),
                ImageThumbnailUrl = imageThumbnailUrl ?? Faker.Url.Image()
            };
    }
}
