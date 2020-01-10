using System;
using System.Collections.Generic;
using System.Linq;

namespace Capibara.Domain.Models
{
    public static partial class ModelFixture
    {
        public static Notification Notification() =>
            new Notification
            {
                Id = Faker.RandomNumber.Next(),
                Title = Faker.Lorem.Sentence(),
                Message = Faker.Lorem.Paragraph(),
                Url = Faker.Url.Root(),
                PublishedAt = Faker.Time.DateTimeOffset()
            };

        public static ICollection<Notification> NotificationCollection(int size = 10) =>
            Enumerable.Range(0, size)
                .Select(_ => ModelFixture.Notification())
                .ToList();
    }
}
