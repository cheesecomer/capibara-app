using System;
using System.Net;

using Capibara.Models;
using Capibara.Net;

using Moq;
using NUnit.Framework;

using SubjectViewModel = Capibara.ViewModels.SplashPageViewModel;

namespace Capibara.Test.ViewModels.SplashPageViewModel
{
    namespace LogoTopMarginPropertyTest
    {
        public class WhenDefault
        {
            SubjectViewModel Subject;

            [SetUp]
            public void SetUp()
            {
                this.Subject = new SubjectViewModel();
            }

            [TestCase]
            public void ItShouldValueWithExpect()
            {
                Assert.That(this.Subject.LogoTopMargin.Value, Is.EqualTo(180));
            }
        }
    }

    namespace LogoOpacityPropertyTest
    {
        public class WhenDefault
        {
            SubjectViewModel Subject;

            [SetUp]
            public void SetUp()
            {
                this.Subject = new SubjectViewModel();
            }

            [TestCase]
            public void ItShouldValueWithExpect()
            {
                Assert.That(this.Subject.LogoOpacity.Value, Is.EqualTo(1));
            }
        }
    }

    namespace LogoScalePropertyTest
    {
        public class WhenDefault
        {
            SubjectViewModel Subject;

            [SetUp]
            public void SetUp()
            {
                this.Subject = new SubjectViewModel();
            }

            [TestCase]
            public void ItShouldValueWithExpect()
            {
                Assert.That(this.Subject.LogoScale.Value, Is.EqualTo(1));
            }
        }
    }

    namespace RefreshCommandTest
    {

        public abstract class TestBase : ViewModelTestBase
        {
            protected abstract string AccessToken { get; }

            protected SubjectViewModel Subject { get; private set; }

            protected virtual User Response { get; }

            protected virtual Exception Exception { get; }

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Subject = new SubjectViewModel(this.NavigationService).BuildUp(this.Container);

                var request = new Mock<RequestBase<User>>();
                if (this.Response.IsPresent())
                    request.Setup(x => x.Execute()).ReturnsAsync(this.Response);
                else if (this.Exception.IsPresent())
                    request.Setup(x => x.Execute()).ThrowsAsync(this.Exception);

                this.RequestFactory.Setup(x => x.UsersShowRequest(It.IsAny<User>())).Returns(request.Object);

                this.IsolatedStorage.UserId = 1;
                this.IsolatedStorage.AccessToken = this.AccessToken;

                this.Subject.RefreshCommand.Execute();
                while (!this.Subject.RefreshCommand.CanExecute()) { }
            }
        }

        public class WhenHasNotAccessToken : TestBase
        {
            protected override string AccessToken => string.Empty;

            [TestCase]
            public void ItShouldLogoScaleWithExpect()
            {
                Assert.That(this.Subject.LogoScale.Value, Is.EqualTo(1));
            }

            [TestCase]
            public void ItShouldLogoOpacityWithExpect()
            {
                Assert.That(this.Subject.LogoOpacity.Value, Is.EqualTo(1));
            }

            [TestCase]
            public void ItShouldLogoTopMarginWithExpect()
            {
                Assert.That(this.Subject.LogoTopMargin.Value, Is.EqualTo(20));
            }

            [TestCase]
            public void ItShouldNavigateToSignUpPage()
            {
                Assert.That(this.NavigatePageName, Is.EqualTo("SignUpPage"));
            }
        }

        public class WhenHasValidAccessTokenAndNotAccepted : TestBase
        {
            protected override string AccessToken => Guid.NewGuid().ToString();

            protected override User Response => new User();

            [TestCase]
            public void ItShouldLogoScaleWithExpect()
            {
                Assert.That(this.Subject.LogoScale.Value, Is.EqualTo(3));
            }

            [TestCase]
            public void ItShouldLogoOpacityWithExpect()
            {
                Assert.That(this.Subject.LogoOpacity.Value, Is.EqualTo(0));
            }

            [TestCase]
            public void ItShouldLogoTopMarginWithExpect()
            {
                Assert.That(this.Subject.LogoTopMargin.Value, Is.EqualTo(180));
            }

            [TestCase]
            public void ItShouldNavigateToAcceptPage()
            {
                Assert.That(this.NavigatePageName, Is.EqualTo("/NavigationPage/AcceptPage"));
            }
        }


        public class WhenHasValidAccessTokenAndAccepted : TestBase
        {
            protected override string AccessToken => Guid.NewGuid().ToString();

            protected override User Response => new User { IsAccepted = true };

            [TestCase]
            public void ItShouldLogoScaleWithExpect()
            {
                Assert.That(this.Subject.LogoScale.Value, Is.EqualTo(3));
            }

            [TestCase]
            public void ItShouldLogoOpacityWithExpect()
            {
                Assert.That(this.Subject.LogoOpacity.Value, Is.EqualTo(0));
            }

            [TestCase]
            public void ItShouldLogoTopMarginWithExpect()
            {
                Assert.That(this.Subject.LogoTopMargin.Value, Is.EqualTo(180));
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
