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
        public class WhenSuccess
        {
            private Block actual;

            [SetUp]
            public void Setup()
            {
                // ISecureIsolatedStorage のセットアップ
                var isolatedStorage = new Mock<IIsolatedStorage>();
                isolatedStorage.SetupAllProperties();
                isolatedStorage.SetupGet(x => x.UserId).Returns(10);

                var container = new UnityContainer();
                container.RegisterInstance<IUnityContainer>(container);
                container.RegisterInstance<IIsolatedStorage>(isolatedStorage.Object);

                var json = "{ \"target\": { \"id\": 10, \"nickname\": \"ABC\" }, \"id\": 99999 }";
                this.actual = JsonConvert.DeserializeObject<Block>(json);

                this.actual.BuildUp(container);
            }

            [TestCase]
            public void ItShouldIdWithExpected()
            {
                Assert.That(this.actual.Id, Is.EqualTo(99999));
            }

            [TestCase]
            public void ItShouldSenderWithExpected()
            {
                Assert.That(this.actual.Target, Is.EqualTo(new User { Id = 10, Nickname = "ABC" }).Using(new UserComparer()));
            }
        }
    }
}
