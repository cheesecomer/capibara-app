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

        public override IUnityContainer GenerateUnityContainer()
        {
            var container = base.GenerateUnityContainer();

            var overrideUrlService = new Mock<IOverrideUrlService>();
            overrideUrlService
                .Setup(x => x.OverrideUrl(It.IsAny<IDeviceService>(), It.IsAny<string[]>()))
                .Returns<IDeviceService, string[]>((x, y) => (IOverrideUrlCommandParameters v) => this.IsOverrideUrlCalled = true);

            container.RegisterInstance(overrideUrlService.Object);

            return container;
        }
    }

    [TestFixture("", "http://example.com/", "http://example.com/", "http://example.com/")]
    [TestFixture(null, "http://example.com/", "http://example.com/", "http://example.com/")]
    [TestFixture("Foo & Bar !!!", "http://example.com/", "Foo & Bar !!!", "http://example.com/")]
    public class OnNavigatedToTest : ViewModelTestBase
    {
        SubjectViewModel Subject;
        string title;
        string url;
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
        public void SetUp()
        {
            this.Subject = new SubjectViewModel();
            this.Subject.BuildUp(this.GenerateUnityContainer());

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
            Assert.That(this.Subject.Source.Value.Url, Is.EqualTo(this.expectUrl));
        }
    }

    public class OverrideUrlCommandTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShoulLoadedPropertyUpdate()
        {
            var viewModel = new SubjectViewModel().BuildUp(this.GenerateUnityContainer());
            viewModel.OnNavigatedTo(new NavigationParameters
            {
                { ParameterNames.Url, "http://foobar.com/" }
            });

            viewModel.OverrideUrlCommand.Execute(new Mock<IOverrideUrlCommandParameters>().Object);

            Assert.That(this.IsOverrideUrlCalled, Is.EqualTo(true));
        }
    }
}
