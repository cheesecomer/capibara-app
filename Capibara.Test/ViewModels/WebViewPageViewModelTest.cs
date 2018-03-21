using System;

using Capibara.ViewModels;

using Prism.Navigation;
using NUnit.Framework;

using SubjectViewModel = Capibara.ViewModels.WebViewPageViewModel;

using Xamarin.Forms;
namespace Capibara.Test.ViewModels.WebViewPageViewModel
{

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
            Assert.That((this.Subject.Source.Value as UrlWebViewSource).Url, Is.EqualTo(this.expectUrl));
        }
    }
}
