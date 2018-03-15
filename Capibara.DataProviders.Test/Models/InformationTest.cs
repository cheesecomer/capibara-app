using System;

using Capibara.Models;
using NUnit.Framework;
using Newtonsoft.Json;
using Moq;
using Microsoft.Practices.Unity;

namespace Capibara.Test.Models.InformationTest
{
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
