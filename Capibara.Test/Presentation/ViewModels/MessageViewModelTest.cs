﻿#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Capibara.Domain.Models;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using Reactive.Bindings.Extensions;

namespace Capibara.Presentation.ViewModels
{
    [TestFixture]
    public class MessageViewModelTest
    {
        static private IEnumerable Property_WhenRisePropertyChanged_ShouldUpdate_TestCase()
        {
            yield return new TestCaseData(
                new Func<Message, object>(x => x.Id = Faker.RandomNumber.Next()),
                new Func<MessageViewModel, object>(x => x.Id.Value))
                .SetName("Id Property When changed Should change");

            yield return new TestCaseData(
                new Func<Message, object>(x => x.Content = Faker.Lorem.Paragraph()),
                new Func<MessageViewModel, object>(x => x.Content.Value))
                .SetName("Content Property When changed Should change");

            yield return new TestCaseData(
                new Func<Message, object>(x => x.At = DateTimeOffset.Now),
                new Func<MessageViewModel, object>(x => x.At.Value))
                .SetName("At Property When changed Should change");

            yield return new TestCaseData(
                new Func<Message, object>(x => x.Sender = ModelFixture.User()),
                new Func<MessageViewModel, object>(x => x.Sender.Value.Model))
                .SetName("Sender Property When changed Should change");

            yield return new TestCaseData(
                new Func<Message, object>(x => x.Sender.IconThumbnailUrl = Faker.Url.Image()),
                new Func<MessageViewModel, object>(x => x.IconThumbnailUrl.Value))
                .SetName("IconThumbnailUrl Property When changed Should change");

            yield return new TestCaseData(
                new Func<Message, object>(x => (x.Sender = ModelFixture.User()).IconThumbnailUrl),
                new Func<MessageViewModel, object>(x => x.IconThumbnailUrl.Value))
                .SetName("IconThumbnailUrl Property When changed Should change User");

            yield return new TestCaseData(
                new Func<Message, object>(x => x.ImageThumbnailUrl = Faker.Url.Image()),
                new Func<MessageViewModel, object>(x => x.ImageThumbnailUrl.Value))
                .SetName("ImageThumbnailUrl Property When changed Should change");
        }

        [Test]
        [TestCaseSource("Property_WhenRisePropertyChanged_ShouldUpdate_TestCase")]
        public void Property_WhenRisePropertyChanged_ShouldUpdate(Func<Message, object> setter, Func<MessageViewModel, object> getter)
        {
            var subject = new MessageViewModel(model: ModelFixture.Message());
            var expected = setter(subject.Model);
            Assert.That(getter(subject), Is.EqualTo(expected));
        }

        static private IEnumerable PropertyChangedTestCase()
        {
            yield return new TestCaseData(
                new Func<MessageViewModel, IObservable<PropertyChangedEventArgs>>(x => x.Id.PropertyChangedAsObservable()),
                new Action<Message>(x => { }), 0)
                .SetName("Id Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                new Func<MessageViewModel, IObservable<PropertyChangedEventArgs>>(x => x.Id.PropertyChangedAsObservable()),
                new Action<Message>(x => x.Id = Faker.RandomNumber.Next()), 1)
                .SetName("Id Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                new Func<MessageViewModel, IObservable<PropertyChangedEventArgs>>(x => x.Content.PropertyChangedAsObservable()),
                new Action<Message>(x => { }), 0)
                .SetName("Content Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                new Func<MessageViewModel, IObservable<PropertyChangedEventArgs>>(x => x.Content.PropertyChangedAsObservable()),
                new Action<Message>(x => x.Content = Faker.Lorem.Paragraph()), 1)
                .SetName("Content Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                new Func<MessageViewModel, IObservable<PropertyChangedEventArgs>>(x => x.At.PropertyChangedAsObservable()),
                new Action<Message>(x => { }), 0)
                .SetName("At Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                new Func<MessageViewModel, IObservable<PropertyChangedEventArgs>>(x => x.At.PropertyChangedAsObservable()),
                new Action<Message>(x => x.At = DateTimeOffset.Now), 1)
                .SetName("At Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                new Func<MessageViewModel, IObservable<PropertyChangedEventArgs>>(x => x.Sender.PropertyChangedAsObservable()),
                new Action<Message>(x => { }), 0)
                .SetName("Sender Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                new Func<MessageViewModel, IObservable<PropertyChangedEventArgs>>(x => x.Sender.PropertyChangedAsObservable()),
                new Action<Message>(x => x.Sender = ModelFixture.User()), 1)
                .SetName("Sender Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                new Func<MessageViewModel, IObservable<PropertyChangedEventArgs>>(x => x.IconThumbnailUrl.PropertyChangedAsObservable()),
                new Action<Message>(x => { }), 0)
                .SetName("IconThumbnailUrl Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                new Func<MessageViewModel, IObservable<PropertyChangedEventArgs>>(x => x.IconThumbnailUrl.PropertyChangedAsObservable()),
                new Action<Message>(x => x.Sender.IconThumbnailUrl = Faker.Url.Image()), 1)
                .SetName("IconThumbnailUrl Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                new Func<MessageViewModel, IObservable<PropertyChangedEventArgs>>(x => x.IconThumbnailUrl.PropertyChangedAsObservable()),
                new Action<Message>(x => x.Sender = ModelFixture.User()), 1)
                .SetName("IconThumbnailUrl Property When chnaged User Should raise PropertyChanged");

            yield return new TestCaseData(
                new Func<MessageViewModel, IObservable<PropertyChangedEventArgs>>(x => x.ImageThumbnailUrl.PropertyChangedAsObservable()),
                new Action<Message>(x => { }), 0)
                .SetName("ImageThumbnailUrl Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                new Func<MessageViewModel, IObservable<PropertyChangedEventArgs>>(x => x.ImageThumbnailUrl.PropertyChangedAsObservable()),
                new Action<Message>(x => x.ImageThumbnailUrl = Faker.Url.Image()), 1)
                .SetName("ImageThumbnailUrl Property When chnaged Should raise PropertyChanged");
        }

        [Test]
        [TestCaseSource("PropertyChangedTestCase")]
        public void PropertyChanged(
            Func<MessageViewModel, IObservable<PropertyChangedEventArgs>> observableGetter,
            Action<Message> setter,
            int expected)
        {
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<PropertyChangedEventArgs>();
            var subject = new MessageViewModel(model: ModelFixture.Message());

            observableGetter(subject)
                .Where(x => x.PropertyName == "Value")
                .Subscribe(observer);

            setter(subject.Model);

            Assert.That(observer.Messages.Count, Is.EqualTo(expected));
        }

        static private IEnumerable OgpItemsTestCase()
        {
            yield return new TestCaseData(
                "see! http://example.com", new string[] { "http://example.com" })
                .SetName("OgpItems When Content Has Url");

            yield return new TestCaseData(
                "see! http://example.com/?id=1", new string[] { "http://example.com/?id=1" })
                .SetName("OgpItems When Content Has Url With query");

            yield return new TestCaseData(
                "see! http://example.com/?id=1&page=2", new string[] { "http://example.com/?id=1&page=2" })
                .SetName("OgpItems When Content Has Url With multi query");

            yield return new TestCaseData(
                "see! http://example.com/?q=%E6%AD%A3%E8%A6%8F%E8%A1%A8%E7%8F%BE&oq=%E6%AD%A3%E8%A6%8F%E8%A1%A8%E7%8F%BE", 
                new string[] 
                { 
                    "http://example.com/?q=%E6%AD%A3%E8%A6%8F%E8%A1%A8%E7%8F%BE&oq=%E6%AD%A3%E8%A6%8F%E8%A1%A8%E7%8F%BE" 
                })
                .SetName("OgpItems When Content Has Url With encoded query");

            yield return new TestCaseData(
                "see! http://example.com/?q=%E6%AD%A3%E8%A6%8F%E8%A1%A8%E7%8F%BE&oq=%E6%AD%A3%E8%A6%8F%E8%A1%A8%E7%8F%BE&p=%E6%AD%A3%E8%A6%8F%E8%A1%A8%E7%8F%BE&oq=%E6%AD%A3%E8%A6%8F%E8%A1%A8%E7%8F%BE&",
                new string[]
                {
                    "http://example.com/?q=%E6%AD%A3%E8%A6%8F%E8%A1%A8%E7%8F%BE&oq=%E6%AD%A3%E8%A6%8F%E8%A1%A8%E7%8F%BE&p=%E6%AD%A3%E8%A6%8F%E8%A1%A8%E7%8F%BE&oq=%E6%AD%A3%E8%A6%8F%E8%A1%A8%E7%8F%BE&"
                })
                .SetName("OgpItems When Content Has Url With multi encoded query");

            yield return new TestCaseData(
                "see! http://example.com http://example.com http://example.com",
                new string[] 
                {
                    "http://example.com",
                    "http://example.com",
                    "http://example.com"
                })
                .SetName("OgpItems When Content Has many Url");

            yield return new TestCaseData(
                "see! http://example.com/index.html", new string[] { "http://example.com/index.html" })
                .SetName("OgpItems When Content Has HTML Url");

            yield return new TestCaseData(
                "see! http://example.com/index.php", new string[] { "http://example.com/index.php" })
                .SetName("OgpItems When Content Has PHP Url");

            yield return new TestCaseData(
                "see! http://example.com/index.py", new string[] { "http://example.com/index.py" })
                .SetName("OgpItems When Content Has Python Url");

            yield return new TestCaseData(
                "see! http://example.com/image.png", new string[] { "http://example.com/image.png" })
                .SetName("OgpItems When Content Has PNG Url");

            yield return new TestCaseData(
                "see! http://example.com/image.jpeg http://example.com/image.jpg", 
                new string[] {
                    "http://example.com/image.jpeg",
                    "http://example.com/image.jpg"
                })
                .SetName("OgpItems When Content Has JPEG Url");

            yield return new TestCaseData(
                "see! http://example.com/image.gif", new string[] { "http://example.com/image.gif" })
                .SetName("OgpItems When Content Has GIF Url");
        }

        [Test]
        [TestCaseSource("OgpItemsTestCase")]
        public void OgpItems(string content, string[] expected)
        {
            var subject = new MessageViewModel(model: ModelFixture.Message(content: content));

            Assert.That(subject.OgpItems.Select(x => x.Model.Url).ToArray(), Is.EqualTo(expected));
        }
    }
}
