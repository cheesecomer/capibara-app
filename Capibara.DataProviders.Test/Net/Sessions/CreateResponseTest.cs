using System;

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
            private CreateResponse Subject;

            [SetUp]
            public void Setup()
            {
                var json = "{ \"access_token\": \"AAA\", \"id\": 999, \"nickname\": \"xxxxx!\", \"biography\":\"...\" }";
                this.Subject = JsonConvert.DeserializeObject<CreateResponse>(json);
            }

            [TestCase]
            public void ItShouldNicknameWithExpect()
            {
                Assert.That(this.Subject.Nickname, Is.EqualTo("xxxxx!"));
            }

            [TestCase]
            public void ItShouldBiographyWithExpect()
            {
                Assert.That(this.Subject.Biography, Is.EqualTo("..."));
            }

            [TestCase]
            public void ItShouldAccessTokenWithExpected()
            {
                Assert.That(this.Subject.AccessToken, Is.EqualTo("AAA"));
            }

            [TestCase]
            public void ItShouldUserIdWithExpected()
            {
                Assert.That(this.Subject.Id, Is.EqualTo(999));
            }
        }
    }
}
