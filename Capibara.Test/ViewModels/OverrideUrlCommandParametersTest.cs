using System;
using System.Linq;

using Capibara.ViewModels;

using Moq;
using NUnit.Framework;

using Prism.Services;

using Xamarin.Forms;

using SubjectClass = Capibara.ViewModels.OverrideUrlCommandParameters;

namespace Capibara.Test.ViewModels.OverrideUrlCommandParameters
{
    public class UrlPropertyTest
    {
        [TestCase("http://foobar.com")]
        public void ItShouldExpect(string url)
        {
            var args = new WebNavigatingEventArgs(WebNavigationEvent.Forward, null, url);
            var subject = new SubjectClass(args) as IOverrideUrlCommandParameters;
            Assert.That(subject.Url, Is.EqualTo(url));
        }
    }

    public class CancelPropertySetTest
    {
        [TestCase(true)]
        [TestCase(false)]
        public void ItShouldExpect(bool cancel)
        {
            var args = new WebNavigatingEventArgs(WebNavigationEvent.Forward, null, "http://foobar.com");

            var subject = new SubjectClass(args) as IOverrideUrlCommandParameters;
            subject.Cancel = cancel;

            Assert.That(args.Cancel, Is.EqualTo(cancel));
        }
    }

    public class CancelPropertyGetTest
    {
        [TestCase(true)]
        [TestCase(false)]
        public void ItShouldExpect(bool cancel)
        {
            var args = new WebNavigatingEventArgs(WebNavigationEvent.Forward, null, "http://foobar.com");
            args.Cancel = cancel;

            var subject = new SubjectClass(args) as IOverrideUrlCommandParameters;

            Assert.That(subject.Cancel, Is.EqualTo(cancel));
        }
    }
}
