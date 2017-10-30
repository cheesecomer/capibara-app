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
            private CreateResponse actual;

            [SetUp]
            public void Setup()
            {
                var json = "{ \"access_token\": \"AAA\", \"user_id\": 999 }";
                this.actual = JsonConvert.DeserializeObject<CreateResponse>(json);
            }

            [TestCase]
            public void ItShouldAccessTokenWithExpected()
            {
                Assert.That(this.actual.AccessToken, Is.EqualTo("AAA"));
            }

            [TestCase]
            public void ItShouldUserIdWithExpected()
            {
                Assert.That(this.actual.UserId, Is.EqualTo(999));
            }
        }
    }
}
