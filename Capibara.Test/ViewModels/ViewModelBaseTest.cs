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
            this.OnFail(() => Task.Run(() => {})).Invoke(null, new FailEventArgs(exception));
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
        public class WhenNavigationParametersIsNull : ViewModelTestBase
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {
                Assert.DoesNotThrow(() => new StabViewModel().BuildUp(this.Container).OnNavigatedTo(null));
            }
        }

        [TestFixture]
        public class WhenNavigationParametersIsEmpty : ViewModelTestBase
        {
            [TestCase]
            public void ItShoulNotThrowException()
            {
                Assert.DoesNotThrow(() => new StabViewModel().BuildUp(this.Container).OnNavigatedTo(new NavigationParameters()));
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
                var parameters = new NavigationParameters { { ParameterNames.Model, model } };

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

                this.Subject = new StabViewModel(this.NavigationService.Object, this.PageDialogService.Object);
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
                this.PageDialogService.Verify(x => x.DisplayAlertAsync("なんてこった！", "再度ログインしてください", "閉じる"), Times.Once());
            }

            [TestCase]
            public void ItShoulGoToSignIn()
            {
                this.NavigationService.Verify(x => x.NavigateAsync("/SignUpPage", null), Times.Once());
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
                this.PageDialogService.Verify(x => x.DisplayAlertAsync("なんてこった！", "不正なアクセスです。再度操作をやり直してください。", "閉じる"), Times.Once());
            }

            [TestCase]
            public void ItShoulGoBack()
            {
                this.NavigationService.Verify(x => x.GoBackAsync(), Times.Once());
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
                this.PageDialogService.Verify(x => x.DisplayAlertAsync("なんてこった！", "データが見つかりません。再度操作をやり直してください。", "閉じる"), Times.Once());
            }

            [TestCase]
            public void ItShoulGoBack()
            {
                this.NavigationService.Verify(x => x.GoBackAsync(), Times.Once());
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
                this.PageDialogService.Verify(x => x.DisplayAlertAsync("なんてこった！", "最新のアプリが公開されています！アップデートを行ってください。", "閉じる"), Times.Once());
            }

            [TestCase]
            public void ItShoulOpenUriCalled()
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
                this.PageDialogService.Verify(x => x.DisplayAlertAsync("申し訳ございません！", "現在メンテナンス中です。時間を置いて再度お試しください。", "閉じる"), Times.Once());
            }

            [TestCase]
            public void ItShoulExit()
            {
                Assert.That(this.IsExitCalled, Is.EqualTo(true));
            }
        }
    }
}

