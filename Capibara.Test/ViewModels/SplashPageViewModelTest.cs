using System;

using System.Net;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;

using Moq;
using NUnit.Framework;

using Prism.Navigation;
using Prism.Services;
namespace Capibara.Test.ViewModels.SplashPageViewModelTest
{
    namespace LogoTopMarginPropertyTest
    {
        public class WhenDefault
        {
            SplashPageViewModel actual;

            [SetUp]
            public void SetUp()
            {
                this.actual = new SplashPageViewModel();
            }

            [TestCase]
            public void ItShouldValueWithExpect()
            {
                Assert.That(this.actual.LogoTopMargin.Value, Is.EqualTo(180));
            }
        }
    }

    namespace LogoOpacityPropertyTest
    {
        public class WhenDefault
        {
            SplashPageViewModel actual;

            [SetUp]
            public void SetUp()
            {
                this.actual = new SplashPageViewModel();
            }

            [TestCase]
            public void ItShouldValueWithExpect()
            {
                Assert.That(this.actual.LogoOpacity.Value, Is.EqualTo(1));
            }
        }
    }

    namespace LogoScalePropertyTest
    {
        public class WhenDefault
        {
            SplashPageViewModel actual;

            [SetUp]
            public void SetUp()
            {
                this.actual = new SplashPageViewModel();
            }

            [TestCase]
            public void ItShouldValueWithExpect()
            {
                Assert.That(this.actual.LogoScale.Value, Is.EqualTo(1));
            }
        }
    }

    namespace RefreshCommandTest
    {

        public abstract class TestBase : ViewModelTestBase
        {
            protected abstract string AccessToken { get; }

            protected string NavigatePageName { get; private set; }

            protected SplashPageViewModel Actual { get; private set; }

            [SetUp]
            public void SetUp()
            {
                var navigationService = new Mock<INavigationService>();
                navigationService
                    .Setup(x => x.NavigateAsync(It.IsAny<string>(), It.IsAny<NavigationParameters>(), It.IsAny<bool?>(), It.IsAny<bool>()))
                    .Returns((string name, NavigationParameters parameters, bool? useModalNavigation, bool animated) =>
                    {
                        this.NavigatePageName = name;
                        return Task.Run(() => { });
                    });

                this.Actual = new SplashPageViewModel(navigationService.Object);
                this.Actual.BuildUp(this.GenerateUnityContainer());

                this.IsolatedStorage.AccessToken = this.AccessToken;

                this.Actual.RefreshCommand.Execute();
                while (!this.Actual.RefreshCommand.CanExecute()) { }
            }
        }

        public class WhenHasNotAccessToken : TestBase
        {
            protected override string AccessToken => string.Empty;

            [TestCase]
            public void ItShouldLogoScaleWithExpect()
            {
                Assert.That(this.Actual.LogoScale.Value, Is.EqualTo(1));
            }

            [TestCase]
            public void ItShouldLogoOpacityWithExpect()
            {
                Assert.That(this.Actual.LogoOpacity.Value, Is.EqualTo(1));
            }

            [TestCase]
            public void ItShouldLogoTopMarginWithExpect()
            {
                Assert.That(this.Actual.LogoTopMargin.Value, Is.EqualTo(20));
            }

            [TestCase]
            public void ItShouldNavigateToSignUpPage()
            {
                Assert.That(this.NavigatePageName, Is.EqualTo("SignUpPage"));
            }
        }

        public class WhenHasAccessToken : TestBase
        {
            protected override string AccessToken => Guid.NewGuid().ToString();

            [TestCase]
            public void ItShouldLogoScaleWithExpect()
            {
                Assert.That(this.Actual.LogoScale.Value, Is.EqualTo(3));
            }

            [TestCase]
            public void ItShouldLogoOpacityWithExpect()
            {
                Assert.That(this.Actual.LogoOpacity.Value, Is.EqualTo(0));
            }

            [TestCase]
            public void ItShouldLogoTopMarginWithExpect()
            {
                Assert.That(this.Actual.LogoTopMargin.Value, Is.EqualTo(180));
            }

            [TestCase]
            public void ItShouldNavigateToFloorMapPage()
            {
                Assert.That(this.NavigatePageName, Is.EqualTo("/MainPage/NavigationPage/FloorMapPage"));
            }
        }
    }
}
