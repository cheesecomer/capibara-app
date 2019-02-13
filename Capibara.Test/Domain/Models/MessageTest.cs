#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Collections;
using System.ComponentModel;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using Reactive.Bindings.Extensions;

namespace Capibara.Domain.Models
{
    [TestFixture]
    public class MessageTest
    {
        static private IEnumerable RestoreTestCase()
        {
            yield return new TestCaseData(
                new Action<Message, Message>((actual, expected) => Assert.That(actual.Restore(expected).Id, Is.EqualTo(expected.Id))))
                .SetName("Restore Should Id is to be restored");

            yield return new TestCaseData(
                new Action<Message, Message>((actual, expected) => Assert.That(actual.Restore(expected).Content, Is.EqualTo(expected.Content))))
                .SetName("Restore Should Content is to be restored");

            yield return new TestCaseData(
                new Action<Message, Message>((actual, expected) => Assert.That(actual.Restore(expected).At, Is.EqualTo(expected.At))))
                .SetName("Restore Should At is to be restored");

            yield return new TestCaseData(
                new Action<Message, Message>((actual, expected) => Assert.That(actual.Restore(expected).Sender, Is.EqualTo(expected.Sender))))
                .SetName("Restore Should Sender is to be restored");

            yield return new TestCaseData(
                new Action<Message, Message>((actual, expected) => Assert.That(actual.Restore(expected).ImageUrl, Is.EqualTo(expected.ImageUrl))))
                .SetName("Restore Should ImageUrl is to be restored");

            yield return new TestCaseData(
                new Action<Message, Message>((actual, expected) => Assert.That(actual.Restore(expected).ImageThumbnailUrl, Is.EqualTo(expected.ImageThumbnailUrl))))
                .SetName("Restore Should ImageThumbnailUrl is to be restored");
        }

        [Test]
        [TestCaseSource("RestoreTestCase")]
        public void Restore(Action<Message, Message> assert)
        {
            assert.Invoke(new Message(), ModelFixture.Message());
        }

        static private IEnumerable PropertyChangedTestCase()
        {
            yield return new TestCaseData(
                "Id", new Action<Message>(x => { }), 0)
                .SetName("Id Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "Id", new Action<Message>(x => x.Id = Faker.RandomNumber.Next()), 1)
                .SetName("Id Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                "Content", new Action<Message>(x => { }), 0)
                .SetName("Content Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "Content", new Action<Message>(x => x.Content = Faker.Lorem.Sentence()), 1)
                .SetName("Content Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                "At", new Action<Message>(x => { }), 0)
                .SetName("At Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "At", new Action<Message>(x => x.At = DateTimeOffset.Now), 1)
                .SetName("At Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                "Sender", new Action<Message>(x => { }), 0)
                .SetName("Sender Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "Sender", new Action<Message>(x => x.Sender = ModelFixture.User()), 1)
                .SetName("Sender Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                "ImageUrl", new Action<Message>(x => { }), 0)
                .SetName("ImageUrl Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "ImageUrl", new Action<Message>(x => x.ImageUrl = Faker.Url.Image()), 1)
                .SetName("ImageUrl Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                "ImageThumbnailUrl", new Action<Message>(x => { }), 0)
                .SetName("ImageThumbnailUrl Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "ImageThumbnailUrl", new Action<Message>(x => x.ImageThumbnailUrl = Faker.Url.Image()), 1)
                .SetName("ImageThumbnailUrl Property When chnaged Should raise PropertyChanged");
        }

        [Test]
        [TestCaseSource("PropertyChangedTestCase")]
        public void PropertyChanged(string propertyName, Action<Message> setter, int expected)
        {
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<PropertyChangedEventArgs>();
            var subject = new Message();
            subject.PropertyChangedAsObservable()
                .Where(x => x.PropertyName == propertyName)
                .Subscribe(observer);

            setter.Invoke(subject);

            Assert.That(observer.Messages.Count, Is.EqualTo(expected));
        }
    }
}
