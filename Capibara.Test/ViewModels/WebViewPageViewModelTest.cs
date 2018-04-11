using System;
using System.Linq;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.Services;
using Capibara.ViewModels;

using Moq;
using NUnit.Framework;

using Prism.Services;
using Prism.Navigation;

using Unity;

using Xamarin.Forms;

using SubjectViewModel = Capibara.ViewModels.WebViewPageViewModel;

namespace Capibara.Test.ViewModels.WebViewPageViewModel
{
    public class ViewModelTestBase : ViewModels.ViewModelTestBase
    {
        public bool IsOverrideUrlCalled;

        protected string url;

        public override void SetUp()
        {
            base.SetUp();

            var overrideUrlService = new Mock<IOverrideUrlService>();
            overrideUrlService
                .Setup(x => x.OverrideUrl(It.IsAny<IDeviceService>(), It.Is<string[]>(v => v[0] == url)))
                .Returns<IDeviceService, string[]>((x, y) => (IOverrideUrlCommandParameters v) => this.IsOverrideUrlCalled = true);

            this.Container.RegisterInstance(overrideUrlService.Object);
        }
    }

    [TestFixture("", "http://example.com/", "http://example.com/", "http://example.com/")]
    [TestFixture(null, "http://example.com/", "http://example.com/", "http://example.com/")]
    [TestFixture("Foo & Bar !!!", "http://example.com/", "Foo & Bar !!!", "http://example.com/")]
    [TestFixture(null, null, null, null)]
    public class OnNavigatedToTest : ViewModelTestBase
    {
        SubjectViewModel Subject;
        string title;
        string expectTitle;
        string expectUrl;

        public OnNavigatedToTest(string title, string url, string expectTitle, string expectUrl)
        {
            this.title = title;
            this.url = url;
            this.expectUrl = expectUrl;
            this.expectTitle = expectTitle;
        }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.Subject = new SubjectViewModel();
            this.Subject.BuildUp(this.Container);

            this.Subject.OnNavigatedTo(new NavigationParameters
            {
                { ParameterNames.Title, this.title },
                { ParameterNames.Url, this.url }
            });
        }

        [TestCase]
        public void ItShouldTitleExpect()
        {
            Assert.That(this.Subject.Title.Value, Is.EqualTo(this.expectTitle));
        }

        [TestCase]
        public void ItShouldUrlExpect()
        {
            Assert.That(this.Subject.Source.Value?.Url, Is.EqualTo(this.expectUrl));
        }
    }

    public class OverrideUrlCommandTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShoulLoadedPropertyUpdate()
        {
            this.url = "http://foobar.com/";
            var viewModel = new SubjectViewModel().BuildUp(this.Container);
            viewModel.OnNavigatedTo(new NavigationParameters
            {
                { ParameterNames.Url, this.url }
            });

            viewModel.OverrideUrlCommand.Execute(new Mock<IOverrideUrlCommandParameters>().Object);

            Assert.That(this.IsOverrideUrlCalled, Is.EqualTo(true));
        }
    }
}
