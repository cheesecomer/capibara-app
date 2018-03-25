using System;
using System.Net;
using System.Threading.Tasks;

using Capibara.Net;
using Capibara.Models;
using Capibara.ViewModels;

using Moq;
using NUnit.Framework;

using Prism.Services;
using Prism.Navigation;

namespace Capibara.Test.ViewModels.ViewModelBase
{

    public class StabModel : ModelBase<StabModel>
    {
        public string Name { get; set; }

        public override void Restore(StabModel model)
        {
            this.Name = model.Name;
        }
    }

    public class StabViewModel : ViewModelBase<StabModel>
    {
        public StabViewModel(INavigationService navigationService = null, IPageDialogService pageDialogService = null, StabModel model = null) 
            : base(navigationService, pageDialogService, model) { }

        public void Fail(Exception exception) {
            this.OnFail(null, new FailEventArgs(exception));
        }
    }

    namespace ModelTest
    {
        [TestFixture]
        public class WhenValueIsNull
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {
                Assert.NotNull(new StabViewModel().Model);
            }
        }

        [TestFixture]
        public class WhenValueIsPresent
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {
                Assert.NotNull(new StabViewModel(model: new StabModel()).Model);
            }
        }
    }

    namespace ContainerTest
    {
        [TestFixture]
        public class WhenValueIsNull
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {
                Assert.DoesNotThrow(() => new StabViewModel().Container = null);
            }
        }

        [TestFixture]
        public class WhenValueIsPresent : TestFixtureBase
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {   
                Assert.DoesNotThrow(() => new StabViewModel().Container = this.Container);
            }
        }
    }

    namespace OnNavigatedFromTest
    {
        [TestFixture]
        public class WhenNavigationParametersIsNull
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {
                Assert.DoesNotThrow(() => new StabViewModel().OnNavigatedFrom(null));
            }
        }

        [TestFixture]
        public class WhenNavigationParametersIsEmpty
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {
                Assert.DoesNotThrow(() => new StabViewModel().OnNavigatedFrom(new NavigationParameters()));
            }
        }
    }

    namespace OnNavigatedToTest
    {
        [TestFixture]
        public class WhenNavigationParametersIsNull
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {
                Assert.DoesNotThrow(() => new StabViewModel().OnNavigatedTo(null));
            }
        }

        [TestFixture]
        public class WhenNavigationParametersIsEmpty
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {
                Assert.DoesNotThrow(() => new StabViewModel().OnNavigatedTo(new NavigationParameters()));
            }
        }
    }

    namespace OnNavigatingToTest
    {
        [TestFixture]
        public class WhenNavigationParametersIsNull
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {
                Assert.DoesNotThrow(() => new StabViewModel().OnNavigatingTo(null));
            }
        }

        [TestFixture]
        public class WhenNavigationParametersIsEmpty
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {
                Assert.DoesNotThrow(() => new StabViewModel().OnNavigatingTo(new NavigationParameters()));
            }
        }

        [TestFixture]
        public class WhenNavigationParametersIncludeModel
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {
                var model = new StabModel();
                var viewModel = new StabViewModel();
                var parameters = new NavigationParameters();
                parameters.Add(ParameterNames.Model, model);

                viewModel.OnNavigatingTo(parameters);

                Assert.That(viewModel.Model.Name, Is.EqualTo(model.Name));
            }
        }
    }

    namespace OnFailTest
    {
        public abstract class ViewModelTestBase : ViewModels.ViewModelTestBase
        {
            protected StabViewModel Subject { get; private set; }

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Subject = new StabViewModel(this.NavigationService, this.PageDialogService.Object);
            }
        }

        public class WhenHttpUnauthorizedException : ViewModelTestBase
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Subject.BuildUp(this.Container).Fail(new HttpUnauthorizedException(HttpStatusCode.Unauthorized, "{\"message\": null}"));
            }

            [TestCase]
            public void ItShoulShowDialog()
            {
                Assert.That(this.IsShowDialog, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShoulGoToSignIn()
            {
                Assert.That(this.NavigatePageName, Is.EqualTo("/SignInPage"));
            }
        }

        public class WhenHttpForbiddenException : ViewModelTestBase
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Subject.BuildUp(this.Container).Fail(new HttpForbiddenException(HttpStatusCode.Forbidden, "{\"message\": null}"));
            }

            [TestCase]
            public void ItShoulShowDialog()
            {
                Assert.That(this.IsShowDialog, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShoulGoBack()
            {
                Assert.That(this.IsGoBackCalled, Is.EqualTo(true));
            }
        }

        public class WhenHttpNotFoundException : ViewModelTestBase
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Subject.BuildUp(this.Container).Fail(new HttpNotFoundException(HttpStatusCode.NotFound, "{\"message\": null}"));
            }

            [TestCase]
            public void ItShoulShowDialog()
            {
                Assert.That(this.IsShowDialog, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShoulGoBack()
            {
                Assert.That(this.IsGoBackCalled, Is.EqualTo(true));
            }
        }

        public class WhenHttpUpgradeRequiredException : ViewModelTestBase
        {
            bool IsOpenUriCalled;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.DeviceService
                    .Setup(x => x.OpenUri(It.Is<Uri>(v => v.ToString() == "http://example.com/store")))
                    .Callback<Uri>(v => this.IsOpenUriCalled = true);

                this.Subject.BuildUp(this.Container).Fail(new HttpUpgradeRequiredException(HttpStatusCode.UpgradeRequired, "{\"message\": null}"));
            }

            [TestCase]
            public void ItShoulShowDialog()
            {
                Assert.That(this.IsShowDialog, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShoulGoBack()
            {
                Assert.That(this.IsOpenUriCalled, Is.EqualTo(true));
            }
        }

        public class WhenHttpServiceUnavailableException : ViewModelTestBase
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Subject.BuildUp(this.Container).Fail(new HttpServiceUnavailableException(HttpStatusCode.ServiceUnavailable, "{\"message\": null}"));
            }

            [TestCase]
            public void ItShoulShowDialog()
            {
                Assert.That(this.IsShowDialog, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShoulExit()
            {
                Assert.That(this.IsExitCalled, Is.EqualTo(true));
            }
        }
    }
}

