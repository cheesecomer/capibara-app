#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Collections;
using NUnit.Framework;

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
            yield return new TestCaseData("http://example.com/", "", "http://example.com/")
                .SetName("UrlPropertyChange When Title is empty Shoul Title is URL");

            yield return new TestCaseData("http://example.com/", null, "http://example.com/")
                .SetName("UrlPropertyChange When Title is null Shoul Title is URL");

            yield return new TestCaseData("HTTP://EXAMPLE.COM/", "", "http://example.com/")
                .SetName("UrlPropertyChange When Title is empty And URL is uppercase Shoul Title is URL lowercase");

            yield return new TestCaseData("HTTP://EXAMPLE.COM/", null, "http://example.com/")
                .SetName("UrlPropertyChange When Title is null And URL is uppercase Shoul Title is URL lowercase");

            yield return new TestCaseData("http://example.com/", "TITLE", "TITLE")
                .SetName("UrlPropertyChange When Title is Present Shoul Title is Title");

            yield return new TestCaseData("HTTP://EXAMPLE.COM/", "TITLE", "TITLE")
                .SetName("UrlPropertyChange When Title is Present And URL is uppercase Shoul Title is Title");
        }

        [Test]
        [TestCaseSource("UrlPropertyChange_WhenTitleIs_ShoulTitleIs_TestCase")]
        public void UrlPropertyChange_WhenTitleIs_ShoulTitleIs(string url, string title, string expected)
        {
            Assert.That(new Ogp { Title = title, Url = url}.Title, Is.EqualTo(expected));
        }
    }
}
