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
    public class OgpTest
    {
        static private IEnumerable RestoreTestCase()
        {
            yield return new TestCaseData(
                new Action<Ogp, Ogp>((actual, expected) => Assert.That(actual.Restore(expected).Url, Is.EqualTo(expected.Url))))
                .SetName("Restore Should Url is to be restored");

            yield return new TestCaseData(
                new Action<Ogp, Ogp>((actual, expected) => Assert.That(actual.Restore(expected).Title, Is.EqualTo(expected.Title))))
                .SetName("Restore Should Title is to be restored");

            yield return new TestCaseData(
                new Action<Ogp, Ogp>((actual, expected) => Assert.That(actual.Restore(expected).Description, Is.EqualTo(expected.Description))))
                .SetName("Restore Should Description is to be restored");

            yield return new TestCaseData(
                new Action<Ogp, Ogp>((actual, expected) => Assert.That(actual.Restore(expected).ImageUrl, Is.EqualTo(expected.ImageUrl))))
                .SetName("Restore Should ImageUrl is to be restored");
        }

        [Test]
        [TestCaseSource("RestoreTestCase")]
        public void Restore(Action<Ogp, Ogp> assert)
        {
            assert.Invoke(new Ogp(), ModelFixture.Ogp());
        }

        static private IEnumerable UrlPropertyChange_WhenTitleIs_ShoulTitleIs_TestCase()
        {
            var url = Faker.Url.Root();
            yield return new TestCaseData(url, "", url.ToLower())
                .SetName("UrlPropertyChange When Title is empty Shoul Title is URL");

            yield return new TestCaseData(url, null, url.ToLower())
                .SetName("UrlPropertyChange When Title is null Shoul Title is URL");

            yield return new TestCaseData(url.ToUpper(), "", url.ToLower())
                .SetName("UrlPropertyChange When Title is empty And URL is uppercase Shoul Title is URL lowercase");

            yield return new TestCaseData(url.ToUpper(), null, url.ToLower())
                .SetName("UrlPropertyChange When Title is null And URL is uppercase Shoul Title is URL lowercase");

            yield return new TestCaseData(url, "TITLE", "TITLE")
                .SetName("UrlPropertyChange When Title is Present Shoul Title is Title");
        }

        [Test]
        [TestCaseSource("UrlPropertyChange_WhenTitleIs_ShoulTitleIs_TestCase")]
        public void UrlPropertyChange_WhenTitleIs_ShoulTitleIs(string url, string title, string expected)
        {
            Assert.That(new Ogp { Title = title, Url = url}.Title, Is.EqualTo(expected));
        }

        static private IEnumerable PropertyChangedTestCase()
        {
            yield return new TestCaseData(
                "Url", new Action<Ogp>(x => { }), 0)
                .SetName("Url Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "Url", new Action<Ogp>(x => x.Url = Faker.Url.Root()), 1)
                .SetName("Url Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                "Title", new Action<Ogp>(x => { }), 0)
                .SetName("Title Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "Title", new Action<Ogp>(x => x.Title = Faker.Lorem.Sentence()), 1)
                .SetName("Title Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                "Description", new Action<Ogp>(x => { }), 0)
                .SetName("Description Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "Description", new Action<Ogp>(x => x.Description = Faker.Lorem.Paragraph()), 1)
                .SetName("Description Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                "ImageUrl", new Action<Ogp>(x => { }), 0)
                .SetName("ImageUrl Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "ImageUrl", new Action<Ogp>(x => x.ImageUrl = Faker.Url.Image()), 1)
                .SetName("ImageUrl Property When chnaged Should raise PropertyChanged");
        }

        [Test]
        [TestCaseSource("PropertyChangedTestCase")]
        public void PropertyChanged(string propertyName, Action<Ogp> setter, int expected)
        {
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<PropertyChangedEventArgs>();
            var subject = new Ogp();
            subject.PropertyChangedAsObservable()
                .Where(x => x.PropertyName == propertyName)
                .Subscribe(observer);

            setter.Invoke(subject);

            Assert.That(observer.Messages.Count, Is.EqualTo(expected));
        }
    }
}
