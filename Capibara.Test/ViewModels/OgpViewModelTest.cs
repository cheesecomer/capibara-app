using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

using AngleSharp;
using AngleSharp.Dom;

using Moq;
using NUnit.Framework;

using SubjectViewModel = Capibara.ViewModels.OgpViewModel;

namespace Capibara.Test.ViewModels.OgpViewModel
{
    namespace RefreshCommandTest
    {
        public class HtmlCollectionMock : IHtmlCollection<IElement>
        {
            List<IElement> elements = new List<IElement>();

            Dictionary<string, IElement> idElementPairs = new Dictionary<string, IElement>();

            IElement IHtmlCollection<IElement>.this[int index] => elements[index];

            IElement IHtmlCollection<IElement>.this[string id] => idElementPairs.ValueOrDefault(id);

            int IHtmlCollection<IElement>.Length => elements.Count;

            IEnumerator<IElement> IEnumerable<IElement>.GetEnumerator()
            {
                return elements.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return elements.GetEnumerator();
            }

            public void Add(IElement element)
            {
                this.elements.Add(element);
            }
        }

        public abstract class TestBase : ViewModelTestBase
        {
            protected SubjectViewModel Subject { get; private set; }

            protected Mock<Services.IBrowsingContext> BrowsingContext { get; private set; }

            protected Mock<IDocument> Document { get; private set; }

            protected HttpStatusCode HttpStatusCode { get; set; } = HttpStatusCode.OK;

            public override void SetUp()
            {
                base.SetUp();

                this.Document = new Mock<IDocument>();

                this.BrowsingContext = new Mock<Services.IBrowsingContext>();
                this.BrowsingContext
                    .Setup(x => x.OpenAsync(It.IsAny<string>()))
                    .ReturnsAsync(this.Document.Object);

                this.Document
                    .Setup(x => x.QuerySelectorAll(It.IsAny<string>()))
                    .Returns(new HtmlCollectionMock());

                this.BrowsingContextFactory
                    .Setup(x => x.Create(It.IsAny<IConfiguration>()))
                    .Returns(this.BrowsingContext.Object);

                this.Document.SetupGet(x => x.StatusCode).Returns(this.HttpStatusCode);

                this.Subject = new SubjectViewModel(url: "http://example.com/");
            }
        }

        [TestFixture(HttpStatusCode.Accepted)]
        [TestFixture(HttpStatusCode.Ambiguous)]
        [TestFixture(HttpStatusCode.BadGateway)]
        [TestFixture(HttpStatusCode.BadRequest)]
        [TestFixture(HttpStatusCode.Conflict)]
        [TestFixture(HttpStatusCode.Continue)]
        [TestFixture(HttpStatusCode.Created)]
        [TestFixture(HttpStatusCode.ExpectationFailed)]
        [TestFixture(HttpStatusCode.Forbidden)]
        [TestFixture(HttpStatusCode.Found)]
        [TestFixture(HttpStatusCode.GatewayTimeout)]
        [TestFixture(HttpStatusCode.Gone)]
        [TestFixture(HttpStatusCode.HttpVersionNotSupported)]
        [TestFixture(HttpStatusCode.InternalServerError)]
        [TestFixture(HttpStatusCode.LengthRequired)]
        [TestFixture(HttpStatusCode.MethodNotAllowed)]
        [TestFixture(HttpStatusCode.Moved)]
        [TestFixture(HttpStatusCode.MovedPermanently)]
        [TestFixture(HttpStatusCode.MultipleChoices)]
        [TestFixture(HttpStatusCode.NoContent)]
        [TestFixture(HttpStatusCode.NonAuthoritativeInformation)]
        [TestFixture(HttpStatusCode.NotAcceptable)]
        [TestFixture(HttpStatusCode.NotFound)]
        [TestFixture(HttpStatusCode.NotImplemented)]
        [TestFixture(HttpStatusCode.NotModified)]
        [TestFixture(HttpStatusCode.PartialContent)]
        [TestFixture(HttpStatusCode.PaymentRequired)]
        [TestFixture(HttpStatusCode.PreconditionFailed)]
        [TestFixture(HttpStatusCode.ProxyAuthenticationRequired)]
        [TestFixture(HttpStatusCode.Redirect)]
        [TestFixture(HttpStatusCode.RedirectKeepVerb)]
        [TestFixture(HttpStatusCode.RedirectMethod)]
        [TestFixture(HttpStatusCode.RequestedRangeNotSatisfiable)]
        [TestFixture(HttpStatusCode.RequestEntityTooLarge)]
        [TestFixture(HttpStatusCode.RequestTimeout)]
        [TestFixture(HttpStatusCode.RequestUriTooLong)]
        [TestFixture(HttpStatusCode.ResetContent)]
        [TestFixture(HttpStatusCode.SeeOther)]
        [TestFixture(HttpStatusCode.ServiceUnavailable)]
        [TestFixture(HttpStatusCode.SwitchingProtocols)]
        [TestFixture(HttpStatusCode.TemporaryRedirect)]
        [TestFixture(HttpStatusCode.Unauthorized)]
        [TestFixture(HttpStatusCode.UnsupportedMediaType)]
        [TestFixture(HttpStatusCode.Unused)]
        [TestFixture(HttpStatusCode.UpgradeRequired)]
        [TestFixture(HttpStatusCode.UseProxy)]
        public class WhenNotOk : TestBase
        {
            public WhenNotOk(HttpStatusCode httpStatusCode)
            {
                this.HttpStatusCode = httpStatusCode;
            }

            public override void SetUp()
            {
                base.SetUp();

                this.Subject.BuildUp(this.Container);
            }

            [TestCase]
            public void ItShouldOpenAsyncCall()
            {
                this.BrowsingContext.Verify(x => x.OpenAsync("http://example.com/"), Times.Once());
            }

            [TestCase]
            public void ItShouldQuerySelectorAllDoesNotExecute()
            {
                this.Document.Verify(x => x.QuerySelectorAll(It.IsAny<string>()), Times.Never());
            }

            [TestCase]
            public void ItShouldTitleIsUrl()
            {
                Assert.That(this.Subject.Title.Value, Is.EqualTo("http://example.com/"));
            }
        }

        public class WhenNotBuildUp : TestBase
        {
            [TestCase]
            public void ItShouldOpenAsyncCall()
            {
                this.BrowsingContext.Verify(x => x.OpenAsync(It.IsAny<string>()), Times.Never());
            }

            [TestCase]
            public void ItShouldQuerySelectorAllDoesNotExecute()
            {
                this.Document.Verify(x => x.QuerySelectorAll(It.IsAny<string>()), Times.Never());
            }

            [TestCase]
            public void ItShouldTitleIsUrl()
            {
                Assert.That(this.Subject.Title.Value, Is.EqualTo("http://example.com/"));
            }
        }

        public class WhenTitleIsPresent : TestBase
        {
            public override void SetUp()
            {
                base.SetUp();

                this.Document.SetupGet(x => x.Title).Returns("FooBar!");

                this.Subject.BuildUp(this.Container);
            }

            [TestCase]
            public void ItShouldOpenAsyncCall()
            {
                this.BrowsingContext.Verify(x => x.OpenAsync(It.IsAny<string>()), Times.Once());
            }

            [TestCase]
            public void ItShouldTitleIsHtmlTitle()
            {
                Assert.That(this.Subject.Title.Value, Is.EqualTo("FooBar!"));
            }
        }

        public class WhenTitleIsEmpty : TestBase
        {

            public override void SetUp()
            {
                base.SetUp();

                this.Subject.BuildUp(this.Container);
            }

            [TestCase]
            public void ItShouldOpenAsyncCall()
            {
                this.BrowsingContext.Verify(x => x.OpenAsync(It.IsAny<string>()), Times.Once());
            }

            [TestCase]
            public void ItShouldTitleIsUrl()
            {
                Assert.That(this.Subject.Title.Value, Is.EqualTo("http://example.com/"));
            }
        }

        public class WhenHasOgTitle : TestBase
        {
            public override void SetUp()
            {
                base.SetUp();

                var htmlCollection = new HtmlCollectionMock();
                var metaOgElement = new Mock<IElement>();

                metaOgElement.Setup(x => x.GetAttribute("content")).Returns("FooBar!");

                htmlCollection.Add(metaOgElement.Object);

                this.Document
                    .Setup(x => x.QuerySelectorAll("meta[property=\"og:title\"]"))
                    .Returns(htmlCollection);

                this.Subject.BuildUp(this.Container);
            }

            [TestCase]
            public void ItShouldOpenAsyncCall()
            {
                this.BrowsingContext.Verify(x => x.OpenAsync(It.IsAny<string>()), Times.Once());
            }

            [TestCase]
            public void ItShouldTitleIsOgTitle()
            {
                Assert.That(this.Subject.Title.Value, Is.EqualTo("FooBar!"));
            }
        }

        public class WhenHasOgDescription : TestBase
        {
            public override void SetUp()
            {
                base.SetUp();

                var htmlCollection = new HtmlCollectionMock();
                var metaOgElement = new Mock<IElement>();

                metaOgElement.Setup(x => x.GetAttribute("content")).Returns("FooBar!");

                htmlCollection.Add(metaOgElement.Object);

                this.Document
                    .Setup(x => x.QuerySelectorAll("meta[property=\"og:description\"]"))
                    .Returns(htmlCollection);

                this.Subject.BuildUp(this.Container);
            }

            [TestCase]
            public void ItShouldOpenAsyncCall()
            {
                this.BrowsingContext.Verify(x => x.OpenAsync(It.IsAny<string>()), Times.Once());
            }

            [TestCase]
            public void ItShouldDescriptionIsOgDescription()
            {
                Assert.That(this.Subject.Description.Value, Is.EqualTo("FooBar!"));
            }
        }

        public class WhenHasOgImage : TestBase
        {
            public override void SetUp()
            {
                base.SetUp();

                var htmlCollection = new HtmlCollectionMock();
                var metaOgElement = new Mock<IElement>();

                metaOgElement.Setup(x => x.GetAttribute("content")).Returns("http://example.com/images/1.png");

                htmlCollection.Add(metaOgElement.Object);

                this.Document
                    .Setup(x => x.QuerySelectorAll("meta[property=\"og:image\"]"))
                    .Returns(htmlCollection);

                this.Subject.BuildUp(this.Container);
            }

            [TestCase]
            public void ItShouldOpenAsyncCall()
            {
                this.BrowsingContext.Verify(x => x.OpenAsync(It.IsAny<string>()), Times.Once());
            }

            [TestCase]
            public void ItShouldDescriptionIsOgDescription()
            {
                this.ImageSourceFactory.Verify(
                    x => x.FromUri(It.Is<Uri>(v => v.AbsoluteUri == "http://example.com/images/1.png")), 
                    Times.Once());
            }
        }
    }
}
