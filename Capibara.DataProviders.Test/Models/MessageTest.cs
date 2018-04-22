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
        public class WhenSuccessIsOwn : TestFixtureBase
        {
            private Message Subject;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                var json = "{ \"sender\": { \"id\": 10, \"nickname\": \"ABC\" }, \"id\": 99999, \"content\": \"FooBar. Yes!Yes!Yeeeeees!\", \"at\":  \"2017-10-28T20:25:20.000+09:00\" }";
                this.Subject = JsonConvert.DeserializeObject<Message>(json);

                this.Subject.BuildUp(this.Container);
                this.IsolatedStorage.UserId = 10;
            }

            [TestCase]
            public void ItShouldContentWithExpected()
            {
                Assert.That(this.Subject.Content, Is.EqualTo("FooBar. Yes!Yes!Yeeeeees!"));
            }

            [TestCase]
            public void ItShouldIdWithExpected()
            {
                Assert.That(this.Subject.Id, Is.EqualTo(99999));
            }

            [TestCase]
            public void ItShouldAtWithExpected()
            {
                Assert.That(this.Subject.At, Is.EqualTo(new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9))));
            }

            [TestCase]
            public void ItShouldSenderWithExpected()
            {
                Assert.That(this.Subject.Sender, Is.EqualTo(new User { Id = 10, Nickname = "ABC" }).Using(new UserComparer()));
            }

            [TestCase]
            public void ItShouldIsOwnWithExpected()
            {
                Assert.That(this.Subject.IsOwn, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenSuccessIsOthers : TestFixtureBase
        {
            private Message Subject;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                var json = "{ \"sender\": { \"id\": 11, \"nickname\": \"ABC\" }, \"id\": 99999, \"content\": \"FooBar. Yes!Yes!Yeeeeees!\", \"at\":  \"2017-10-28T20:25:20.000+09:00\" }";
                this.Subject = JsonConvert.DeserializeObject<Message>(json);

                this.Subject.BuildUp(this.Container);
                this.IsolatedStorage.UserId = 10;
            }

            [TestCase]
            public void ItShouldContentWithExpected()
            {
                Assert.That(this.Subject.Content, Is.EqualTo("FooBar. Yes!Yes!Yeeeeees!"));
            }

            [TestCase]
            public void ItShouldIdWithExpected()
            {
                Assert.That(this.Subject.Id, Is.EqualTo(99999));
            }

            [TestCase]
            public void ItShouldAtWithExpected()
            {
                Assert.That(this.Subject.At, Is.EqualTo(new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9))));
            }

            [TestCase]
            public void ItShouldSenderWithExpected()
            {
                Assert.That(this.Subject.Sender, Is.EqualTo(new User { Id = 11, Nickname = "ABC" }).Using(new UserComparer()));
            }

            [TestCase]
            public void ItShouldIsOwnWithExpected()
            {
                Assert.That(this.Subject.IsOwn, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenSuccessSenderEmpty : TestFixtureBase
        {
            private Message Subject;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                var json = "{ \"sender\": null, \"id\": 99999, \"content\": \"FooBar. Yes!Yes!Yeeeeees!\", \"at\":  \"2017-10-28T20:25:20.000+09:00\" }";
                this.Subject = JsonConvert.DeserializeObject<Message>(json);

                this.Subject.BuildUp(this.Container);
                this.IsolatedStorage.UserId = 10;
            }

            [TestCase]
            public void ItShouldContentWithExpected()
            {
                Assert.That(this.Subject.Content, Is.EqualTo("FooBar. Yes!Yes!Yeeeeees!"));
            }

            [TestCase]
            public void ItShouldIdWithExpected()
            {
                Assert.That(this.Subject.Id, Is.EqualTo(99999));
            }

            [TestCase]
            public void ItShouldAtWithExpected()
            {
                Assert.That(this.Subject.At, Is.EqualTo(new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9))));
            }

            [TestCase]
            public void ItShouldSenderWithExpected()
            {
                Assert.That(this.Subject.Sender, Is.EqualTo(null));
            }

            [TestCase]
            public void ItShouldIsOwnWithExpected()
            {
                Assert.That(this.Subject.IsOwn, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenHasImage : TestFixtureBase
        {
            private Message Subject;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                var json = "{ \"sender\": { \"id\": 10, \"nickname\": \"ABC\" }, \"id\": 99999, \"image_url\": \"http://unit-test.com/example.jpg\", \"image_thumb_url\": \"http://unit-test.com/thumb-example.jpg\", \"at\":  \"2017-10-28T20:25:20.000+09:00\" }";
                this.Subject = JsonConvert.DeserializeObject<Message>(json);

                this.Subject.BuildUp(this.Container);
                this.IsolatedStorage.UserId = 10;
            }

            [TestCase]
            public void ItShouldImageUrlWithExpected()
            {
                Assert.That(this.Subject.ImageUrl, Is.EqualTo("http://unit-test.com/example.jpg"));
            }

            [TestCase]
            public void ItShouldImageThumbnailUrlWithExpected()
            {
                Assert.That(this.Subject.ImageThumbnailUrl, Is.EqualTo("http://unit-test.com/thumb-example.jpg"));
            }

            [TestCase]
            public void ItShouldIdWithExpected()
            {
                Assert.That(this.Subject.Id, Is.EqualTo(99999));
            }

            [TestCase]
            public void ItShouldAtWithExpected()
            {
                Assert.That(this.Subject.At, Is.EqualTo(new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9))));
            }

            [TestCase]
            public void ItShouldSenderWithExpected()
            {
                Assert.That(this.Subject.Sender, Is.EqualTo(new User { Id = 10, Nickname = "ABC" }).Using(new UserComparer()));
            }

            [TestCase]
            public void ItShouldIsOwnWithExpected()
            {
                Assert.That(this.Subject.IsOwn, Is.EqualTo(true));
            }
        }
    }
}
