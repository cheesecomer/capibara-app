﻿using System;
namespace Capibara.Domain.Models
{
    public static partial class ModelFixture
    {
        public static User User(
            int? id = null,
            string nickname = null,
            string biography = null,
            string iconUrl = null,
            string iconThumbnailUrl = null,
            int? blockId = null,
            int? followId = null,
            int? friendsCount = null,
            bool? isAccepted = null,
            bool? isFollower = null) => new User
            {
                Id = id ?? Faker.RandomNumber.Next(),
                Nickname = nickname ?? Faker.Name.FullName(),
                Biography = biography ?? Faker.Lorem.Sentence(),
                IconUrl = iconUrl ?? Faker.Url.Image(),
                IconThumbnailUrl = iconThumbnailUrl ?? Faker.Url.Image(),
                BlockId = blockId ?? Faker.RandomNumber.Next(),
                FollowId = followId ?? Faker.RandomNumber.Next(),
                FriendsCount = friendsCount ?? Faker.RandomNumber.Next(),
                IsAccepted = isAccepted ?? true,
                IsFollower = isFollower ?? false
            };
    }
}