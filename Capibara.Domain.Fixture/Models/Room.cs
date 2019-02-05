using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Capibara.Domain.Models
{
    public static partial class ModelFixture
    {
        public static Room Room(
            int? id = null,
            bool? isConnected = null,
            string name = null,
            int? capacity = null,
            ICollection<Message> messages = null,
            ICollection<User> participants = null)
        {
            var fixture = new Room
            {
                Id = id ?? Faker.RandomNumber.Next(),
                IsConnected = isConnected ?? false,
                Name = name ?? Faker.Lorem.Words(1).First(),
                Capacity = capacity ?? Faker.RandomNumber.Next(10, 50),
                NumberOfParticipants = participants?.Count ?? 10
            };

            (messages ?? Enumerable.Range(0, 10).Select(_ => ModelFixture.Message())).ForEach(x => fixture.Messages.Add(x));
            (participants ?? Enumerable.Range(0, 10).Select(_ => ModelFixture.User())).ForEach(x => fixture.Participants.Add(x));

            return fixture;
        }
    }
}
