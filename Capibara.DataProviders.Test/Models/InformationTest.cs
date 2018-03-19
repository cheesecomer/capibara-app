using System;

using Capibara.Models;
using NUnit.Framework;
using Newtonsoft.Json;
using Moq;
using Unity;

namespace Capibara.Test.Models.InformationTest
{
    [TestFixture]
    public class RestoreTest
    {
        private Information Actual;

        [SetUp]
        public void Setup()
        {
            this.Actual = new Information { Id = 99999 };
            this.Actual.Restore(new Information { Id = 99999, Message = "FooBar. Yes!Yes!Yeeeeees!", Title = "...", PublishedAt = new DateTimeOffset(2018, 3, 10, 11, 0, 0, TimeSpan.FromHours(9)) });
        }

        [TestCase]
        public void ItShouldMessageWithExpected()
        {
            Assert.That(this.Actual.Message, Is.EqualTo("FooBar. Yes!Yes!Yeeeeees!"));
        }

        [TestCase]
        public void ItShouldIdWithExpected()
        {
            Assert.That(this.Actual.Id, Is.EqualTo(99999));
        }

        [TestCase]
        public void ItShouldBiographyWithExpected()
        {
            Assert.That(this.Actual.Title, Is.EqualTo("..."));
        }

        [TestCase]
        public void ItShouldPublishedAtWithExpected()
        {
            Assert.That(this.Actual.PublishedAt, Is.EqualTo(new DateTimeOffset(2018, 3, 10, 11, 0, 0, TimeSpan.FromHours(9))));
        }
    }

    namespace DeserializeTest
    {
        [TestFixture]
        public class WhenSuccess
        {
            private Information actual;

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

                var json = "{ \"id\": 99999, \"title\": \"Title!!!\", \"message\": \"Message!!!!\", \"published_at\": \"2017-10-28T20:25:20.000+09:00\"}";
                this.actual = JsonConvert.DeserializeObject<Information>(json);

                this.actual.BuildUp(container);
            }

            [TestCase]
            public void ItShouldIdWithExpected()
            {
                Assert.That(this.actual.Id, Is.EqualTo(99999));
            }

            [TestCase]
            public void ItShouldMessageWithExpected()
            {
                Assert.That(this.actual.Message, Is.EqualTo("Message!!!!"));
            }

            [TestCase]
            public void ItShouldTitleWithExpected()
            {
                Assert.That(this.actual.Title, Is.EqualTo("Title!!!"));
            }

            [TestCase]
            public void ItShouldPublishedAtWithExpected()
            {
                Assert.That(this.actual.PublishedAt, Is.EqualTo(new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9))));
            }
        }
    }
}
