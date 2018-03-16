using System;

using Capibara.Models;
using NUnit.Framework;
using Newtonsoft.Json;
using Moq;
using Unity;

namespace Capibara.Test.Models.MessageTest
{
    namespace DeserializeTest
    {
        [TestFixture]
        public class WhenSuccessIsOwn
        {
            private Message actual;

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

                var json = "{ \"sender\": { \"id\": 10, \"nickname\": \"ABC\" }, \"id\": 99999, \"content\": \"FooBar. Yes!Yes!Yeeeeees!\", \"at\":  \"2017-10-28T20:25:20.000+09:00\" }";
                this.actual = JsonConvert.DeserializeObject<Message>(json);

                this.actual.BuildUp(container);
            }

            [TestCase]
            public void ItShouldNameWithExpected()
            {
                Assert.That(this.actual.Content, Is.EqualTo("FooBar. Yes!Yes!Yeeeeees!"));
            }

            [TestCase]
            public void ItShouldIdWithExpected()
            {
                Assert.That(this.actual.Id, Is.EqualTo(99999));
            }

            [TestCase]
            public void ItShouldAtWithExpected()
            {
                Assert.That(this.actual.At, Is.EqualTo(new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9))));
            }

            [TestCase]
            public void ItShouldSenderWithExpected()
            {
                Assert.That(this.actual.Sender, Is.EqualTo(new User { Id = 10, Nickname = "ABC" }).Using(new UserComparer()));
            }

            [TestCase]
            public void ItShouldIsOwnWithExpected()
            {
                Assert.That(this.actual.IsOwn, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenSuccessIsOthers
        {
            private Message actual;

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

                var json = "{ \"sender\": { \"id\": 11, \"nickname\": \"ABC\" }, \"id\": 99999, \"content\": \"FooBar. Yes!Yes!Yeeeeees!\", \"at\":  \"2017-10-28T20:25:20.000+09:00\" }";
                this.actual = JsonConvert.DeserializeObject<Message>(json);

                this.actual.BuildUp(container);
            }

            [TestCase]
            public void ItShouldNameWithExpected()
            {
                Assert.That(this.actual.Content, Is.EqualTo("FooBar. Yes!Yes!Yeeeeees!"));
            }

            [TestCase]
            public void ItShouldIdWithExpected()
            {
                Assert.That(this.actual.Id, Is.EqualTo(99999));
            }

            [TestCase]
            public void ItShouldAtWithExpected()
            {
                Assert.That(this.actual.At, Is.EqualTo(new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9))));
            }

            [TestCase]
            public void ItShouldSenderWithExpected()
            {
                Assert.That(this.actual.Sender, Is.EqualTo(new User { Id = 11, Nickname = "ABC" }).Using(new UserComparer()));
            }

            [TestCase]
            public void ItShouldIsOwnWithExpected()
            {
                Assert.That(this.actual.IsOwn, Is.EqualTo(false));
            }
        }
    }
}
