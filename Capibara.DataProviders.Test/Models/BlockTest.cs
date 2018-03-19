using System;

using Capibara.Models;
using NUnit.Framework;
using Newtonsoft.Json;
using Moq;
using Unity;

namespace Capibara.Test.Models.BlockTest
{
    namespace DeserializeTest
    {
        [TestFixture]
        public class WhenSuccess : TestFixtureBase
        {
            private Block Actual;

            [SetUp]
            public void Setup()
            {
                var json = "{ \"target\": { \"id\": 10, \"nickname\": \"ABC\" }, \"id\": 99999 }";
                this.Actual = JsonConvert.DeserializeObject<Block>(json);
                this.Actual.BuildUp(this.GenerateUnityContainer());
                this.IsolatedStorage.UserId = 10;
            }

            [TestCase]
            public void ItShouldIdWithExpected()
            {
                Assert.That(this.Actual.Id, Is.EqualTo(99999));
            }

            [TestCase]
            public void ItShouldSenderWithExpected()
            {
                Assert.That(this.Actual.Target, Is.EqualTo(new User { Id = 10, Nickname = "ABC" }).Using(new UserComparer()));
            }
        }
    }
}
