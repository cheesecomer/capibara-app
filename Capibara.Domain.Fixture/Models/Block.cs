using System;
namespace Capibara.Domain.Models
{
    public static partial class ModelFixture
    {
        public static Block Block(
            int? id = null,
            User target = null) => new Block
            {
                Id = id ?? Faker.RandomNumber.Next(),
                Target = target ?? ModelFixture.User()
            };
    }
}
