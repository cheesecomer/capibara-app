﻿using System;

using Capibara.Net.Sessions;

using NUnit.Framework;
using Newtonsoft.Json;

namespace Capibara.Test.Net.Sessions.CreateResponseTest
{
    namespace CreateResponseTest
    {
        [TestFixture]
        public class WhenSuccess
        {
            private CreateResponse Actual;

            [SetUp]
            public void Setup()
            {
                var json = "{ \"access_token\": \"AAA\", \"user_id\": 999, \"user_nickname\": \"xxxxx!\", \"user_biography\":\"...\" }";
                this.Actual = JsonConvert.DeserializeObject<CreateResponse>(json);
            }

            [TestCase]
            public void IsShouldNicknameWithExpect()
            {
                Assert.That(this.Actual.Nickname, Is.EqualTo("xxxxx!"));
            }

            [TestCase]
            public void IsShouldBiographyWithExpect()
            {
                Assert.That(this.Actual.Biography, Is.EqualTo("..."));
            }

            [TestCase]
            public void ItShouldAccessTokenWithExpected()
            {
                Assert.That(this.Actual.AccessToken, Is.EqualTo("AAA"));
            }

            [TestCase]
            public void ItShouldUserIdWithExpected()
            {
                Assert.That(this.Actual.UserId, Is.EqualTo(999));
            }
        }
    }
}
