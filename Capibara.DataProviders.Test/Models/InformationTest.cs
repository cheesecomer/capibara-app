using System;

using Capibara.Models;
using NUnit.Framework;
using Newtonsoft.Json;
using Moq;
using Unity;

namespace Capibara.Test.Models.InformationTest
{
    [TestFixture]
    public class RestoreTest : TestFixtureBase
    {
        private Information Subject;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.Subject = new Information { Id = 99999 };
            this.Subject.Restore(new Information { Id = 99999, Message = "FooBar. Yes!Yes!Yeeeeees!", Title = "...", PublishedAt = new DateTimeOffset(2018, 3, 10, 11, 0, 0, TimeSpan.FromHours(9)), Url = "http://example.com/informations/1" });
        }

        [TestCase]
        public void ItShouldMessageWithExpected()
        {
            Assert.That(this.Subject.Message, Is.EqualTo("FooBar. Yes!Yes!Yeeeeees!"));
        }

        [TestCase]
        public void ItShouldIdWithExpected()
        {
            Assert.That(this.Subject.Id, Is.EqualTo(99999));
        }

        [TestCase]
        public void ItShouldBiographyWithExpected()
        {
            Assert.That(this.Subject.Title, Is.EqualTo("..."));
        }

        [TestCase]
        public void ItShouldPublishedAtWithExpected()
        {
            Assert.That(this.Subject.PublishedAt, Is.EqualTo(new DateTimeOffset(2018, 3, 10, 11, 0, 0, TimeSpan.FromHours(9))));
        }

        [TestCase]
        public void ItShouldUrlWithExpected()
        {
            Assert.That(this.Subject.Url, Is.EqualTo("http://example.com/informations/1"));
        }
    }

    namespace DeserializeTest
    {
        [TestFixture]
        public class WhenSuccess : TestFixtureBase
        {
            private Information Subject;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                var json = "{ \"id\": 99999, \"title\": \"Title!!!\", \"message\": \"Message!!!!\", \"published_at\": \"2017-10-28T20:25:20.000+09:00\", \"url\": \"http://example.com/informations/1\"}";
                this.Subject = JsonConvert.DeserializeObject<Information>(json);
            }

            [TestCase]
            public void ItShouldIdWithExpected()
            {
                Assert.That(this.Subject.Id, Is.EqualTo(99999));
            }

            [TestCase]
            public void ItShouldMessageWithExpected()
            {
                Assert.That(this.Subject.Message, Is.EqualTo("Message!!!!"));
            }

            [TestCase]
            public void ItShouldTitleWithExpected()
            {
                Assert.That(this.Subject.Title, Is.EqualTo("Title!!!"));
            }

            [TestCase]
            public void ItShouldPublishedAtWithExpected()
            {
                Assert.That(this.Subject.PublishedAt, Is.EqualTo(new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9))));
            }

            [TestCase]
            public void ItShouldUrlWithExpected()
            {
                Assert.That(this.Subject.Url, Is.EqualTo("http://example.com/informations/1"));
            }
        }
    }
}
