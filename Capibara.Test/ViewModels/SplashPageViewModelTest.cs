using System;

using System.Net;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;
using Capibara.Net;
using Capibara.Net.Users;

using Moq;
using NUnit.Framework;

using Prism.Navigation;

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

            protected SplashPageViewModel Actual { get; private set; }

            protected virtual User Response { get; }

            protected virtual Exception Exception { get; }

            [SetUp]
            public void SetUp()
            {
                
                this.Actual = new SplashPageViewModel(this.NavigationService).BuildUp(this.GenerateUnityContainer());

                var request = new Mock<RequestBase<User>>();
                if (this.Response.IsPresent())
                    request.Setup(x => x.Execute()).ReturnsAsync(this.Response);
                else if (this.Exception.IsPresent())
                    request.Setup(x => x.Execute()).ThrowsAsync(this.Exception);

                this.RequestFactory.Setup(x => x.UsersShowRequest(It.IsAny<User>())).Returns(request.Object);

                this.IsolatedStorage.UserId = 1;
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

        public class WhenHasValidAccessToken : TestBase
        {
            protected override string AccessToken => Guid.NewGuid().ToString();

            protected override User Response => new User();

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

        public class WhenHasInvalidAccessToken : WhenHasNotAccessToken
        {
            protected override Exception Exception => new HttpUnauthorizedException(HttpStatusCode.Unauthorized, string.Empty);

            protected override string AccessToken => Guid.NewGuid().ToString();
        }
    }
}
