#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Threading.Tasks;
using System.Collections;
using Moq;
using Capibara.Domain.UseCases;
using NUnit.Framework;

namespace Capibara.Presentation.ViewModels
{
    [TestFixture]
    public class AcceptPageViewModelTest
    {
        #region WhenInitialize

        [Test]
        public void Title_WhenInitialize_ShouldIsEmpty()
        {
            Assert.IsEmpty(new AcceptPageViewModel().Title.Value);
        }

        [Test]
        public void CurrentCommand_WhenInitialize_ShouldIsNull()
        {
            Assert.Null(new AcceptPageViewModel().CurrentCommand.Value);
        }

        [Test]
        public void CurrentCommandTitle_WhenInitialize_ShouldIsEmpty()
        {
            Assert.IsEmpty(new AcceptPageViewModel().CurrentCommandTitle.Value);
        }

        [Test]
        public void Url_WhenInitialize_ShouldIsEmpty()
        {
            Assert.IsEmpty(new AcceptPageViewModel().Url.Value);
        }

        #endregion

        #region WhenOpen

        [Test]
        public void OpenCommand_WhenExecute_ShouldInvokeGetWebPageUrlUseCase()
        {
            var termsUrl = Faker.Url.Root();
            var getWebPageUrlUseCase = new Mock<IGetWebPageUrlUseCase>();
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var subject = new AcceptPageViewModel
            {
                SchedulerProvider = schedulerProvider,
                GetWebPageUrlUseCase = getWebPageUrlUseCase.Object
            };

            getWebPageUrlUseCase.Setup(x => x.Invoke(Domain.Models.WebPage.Terms)).ReturnsAsync(termsUrl);

            subject.OpenCommand.Execute();

            getWebPageUrlUseCase.Verify(x => x.Invoke(Domain.Models.WebPage.Terms), Times.Once);
        }

        public static IEnumerable WhenOpenTestCaseSource()
        {
            yield return new TestCaseData(
                new Action<AcceptPageViewModel, string>((x, _) => Assert.That(x.Title.Value, Is.EqualTo("利用規約の同意"))))
                .SetName("Title WhenOpen ShouldIsAgreeToTerms");

            yield return new TestCaseData(
                new Action<AcceptPageViewModel, string>((x, _) => Assert.False(x.CurrentCommand.Value.CanExecute())))
                .SetName("CurrentCommand WhenOpen ShouldCanNotExecute");

            yield return new TestCaseData(
                new Action<AcceptPageViewModel, string>((x, _) => Assert.That(x.CurrentCommandTitle.Value, Is.EqualTo("次へ"))))
                .SetName("CurrentCommandTitle WhenOpen ShouldIsNext");

            yield return new TestCaseData(
                new Action<AcceptPageViewModel, string>((x, y) => Assert.That(x.Url.Value, Is.EqualTo(y))))
                .SetName("Url WhenOpen ShouldIsTermsURL");
        }

        [Test]
        [TestCaseSource("WhenOpenTestCaseSource")]
        public void WhenOpen(Action<AcceptPageViewModel, string> assert)
        {
            var termsUrl = Faker.Url.Root();
            var getWebPageUrlUseCase = new Mock<IGetWebPageUrlUseCase>();
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var subject = new AcceptPageViewModel
            {
                SchedulerProvider = schedulerProvider,
                GetWebPageUrlUseCase = getWebPageUrlUseCase.Object
            };

            getWebPageUrlUseCase.Setup(x => x.Invoke(Domain.Models.WebPage.Terms)).ReturnsAsync(termsUrl);

            subject.OpenCommand.Execute();

            scheduler.AdvanceBy(1);

            scheduler.AdvanceBy(1);

            assert.Invoke(subject, termsUrl);
        }

        #endregion

        #region WhenTemrsLoaded

        [Test]
        public void CurrentCommand_WhenTemrsLoaded_ShouldCanExecute()
        {
            var termsUrl = Faker.Url.Root();
            var getWebPageUrlUseCase = new Mock<IGetWebPageUrlUseCase>();
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var subject = new AcceptPageViewModel
            {
                SchedulerProvider = schedulerProvider,
                GetWebPageUrlUseCase = getWebPageUrlUseCase.Object
            };

            getWebPageUrlUseCase.Setup(x => x.Invoke(Domain.Models.WebPage.Terms)).ReturnsAsync(termsUrl);

            subject.OpenCommand.Execute();

            scheduler.AdvanceBy(1);

            scheduler.AdvanceBy(1);

            Assert.False(subject.CurrentCommand.Value.CanExecute());

            subject.LoadedCommand.Execute();

            scheduler.AdvanceBy(1);

            Assert.True(subject.CurrentCommand.Value.CanExecute());
        }

        #endregion

        #region WhenNext

        [Test]
        public void NextCommand_WhenExecute_ShouldInvokeGetWebPageUrlUseCase()
        {
            var termsUrl = Faker.Url.Root();
            var privacyPolicyUrl = Faker.Url.Root();
            var getWebPageUrlUseCase = new Mock<IGetWebPageUrlUseCase>();
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var subject = new AcceptPageViewModel
            {
                SchedulerProvider = schedulerProvider,
                GetWebPageUrlUseCase = getWebPageUrlUseCase.Object
            };

            getWebPageUrlUseCase.Setup(x => x.Invoke(Domain.Models.WebPage.Terms)).ReturnsAsync(termsUrl);
            getWebPageUrlUseCase.Setup(x => x.Invoke(Domain.Models.WebPage.PrivacyPolicy)).ReturnsAsync(privacyPolicyUrl);

            subject.OpenCommand.Execute();

            scheduler.AdvanceBy(1);

            scheduler.AdvanceBy(1);

            subject.LoadedCommand.Execute();

            scheduler.AdvanceBy(1);

            subject.CurrentCommand.Value.Execute();

            getWebPageUrlUseCase.Verify(x => x.Invoke(Domain.Models.WebPage.PrivacyPolicy), Times.Once);
        }

        public static IEnumerable WhenNextTestCaseSource()
        {
            yield return new TestCaseData(
                new Action<AcceptPageViewModel, string>((x, _) => Assert.That(x.Title.Value, Is.EqualTo("プライバシーポリシーの同意"))))
                .SetName("Title WhenNext ShouldIsAgreeToPrivacyPolicy");

            yield return new TestCaseData(
                new Action<AcceptPageViewModel, string>((x, _) => Assert.False(x.CurrentCommand.Value.CanExecute())))
                .SetName("CurrentCommand WhenNext ShouldCanNotExecute");

            yield return new TestCaseData(
                new Action<AcceptPageViewModel, string>((x, _) => Assert.That(x.CurrentCommandTitle.Value, Is.EqualTo("同意する"))))
                .SetName("CurrentCommandTitle WhenNext ShouldIsAgree");

            yield return new TestCaseData(
                new Action<AcceptPageViewModel, string>((x, y) => Assert.That(x.Url.Value, Is.EqualTo(y))))
                .SetName("Url WhenNext ShouldIsPrivacyPolicyURL");
        }

        [Test]
        [TestCaseSource("WhenNextTestCaseSource")]
        public void WhenNext(Action<AcceptPageViewModel, string> assert)
        {
            var termsUrl = Faker.Url.Root();
            var privacyPolicyUrl = Faker.Url.Root();
            var getWebPageUrlUseCase = new Mock<IGetWebPageUrlUseCase>();
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var subject = new AcceptPageViewModel
            {
                SchedulerProvider = schedulerProvider,
                GetWebPageUrlUseCase = getWebPageUrlUseCase.Object
            };

            getWebPageUrlUseCase.Setup(x => x.Invoke(Domain.Models.WebPage.Terms)).ReturnsAsync(termsUrl);
            getWebPageUrlUseCase.Setup(x => x.Invoke(Domain.Models.WebPage.PrivacyPolicy)).ReturnsAsync(privacyPolicyUrl);

            subject.OpenCommand.Execute();

            scheduler.AdvanceBy(1);

            scheduler.AdvanceBy(1);

            subject.LoadedCommand.Execute();

            scheduler.AdvanceBy(1);

            subject.CurrentCommand.Value.Execute();

            scheduler.AdvanceBy(1);

            scheduler.AdvanceBy(1);

            assert.Invoke(subject, privacyPolicyUrl);
        }

        #endregion

        #region WhenPrivacyPolicyLoaded

        [Test]
        public void CurrentCommand_WhenPrivacyPolicyLoaded_ShouldCanExecute()
        {
            var termsUrl = Faker.Url.Root();
            var privacyPolicyUrl = Faker.Url.Root();
            var getWebPageUrlUseCase = new Mock<IGetWebPageUrlUseCase>();
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var subject = new AcceptPageViewModel
            {
                SchedulerProvider = schedulerProvider,
                GetWebPageUrlUseCase = getWebPageUrlUseCase.Object
            };

            getWebPageUrlUseCase.Setup(x => x.Invoke(Domain.Models.WebPage.Terms)).ReturnsAsync(termsUrl);
            getWebPageUrlUseCase.Setup(x => x.Invoke(Domain.Models.WebPage.PrivacyPolicy)).ReturnsAsync(privacyPolicyUrl);

            subject.OpenCommand.Execute();

            scheduler.AdvanceBy(1);

            scheduler.AdvanceBy(1);

            Assert.False(subject.CurrentCommand.Value.CanExecute());

            subject.LoadedCommand.Execute();

            scheduler.AdvanceBy(1);

            Assert.True(subject.CurrentCommand.Value.CanExecute());

            subject.CurrentCommand.Value.Execute();

            scheduler.AdvanceBy(1);

            scheduler.AdvanceBy(1);

            Assert.False(subject.CurrentCommand.Value.CanExecute());

            subject.LoadedCommand.Execute();

            scheduler.AdvanceBy(1);

            Assert.True(subject.CurrentCommand.Value.CanExecute());
        }

        #endregion


        #region AgreeCommand

        [Test]
        public void AgreeCommand_WhenExecute_ShouldInvokeAcceptUseCase()
        {
            var termsUrl = Faker.Url.Root();
            var privacyPolicyUrl = Faker.Url.Root();
            var getWebPageUrlUseCase = new Mock<IGetWebPageUrlUseCase>();
            var acceptUseCase = new Mock<IAcceptUseCase>();
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var subject = new AcceptPageViewModel
            {
                SchedulerProvider = schedulerProvider,
                GetWebPageUrlUseCase = getWebPageUrlUseCase.Object,
                AcceptUseCase = acceptUseCase.Object
            };

            getWebPageUrlUseCase.Setup(x => x.Invoke(Domain.Models.WebPage.Terms)).ReturnsAsync(termsUrl);
            getWebPageUrlUseCase.Setup(x => x.Invoke(Domain.Models.WebPage.PrivacyPolicy)).ReturnsAsync(privacyPolicyUrl);

            subject.OpenCommand.Execute();

            scheduler.AdvanceBy(1);

            scheduler.AdvanceBy(1);

            subject.LoadedCommand.Execute();

            scheduler.AdvanceBy(1);

            subject.CurrentCommand.Value.Execute();

            scheduler.AdvanceBy(1);

            scheduler.AdvanceBy(1);

            subject.LoadedCommand.Execute();

            scheduler.AdvanceBy(1);

            subject.CurrentCommand.Value.Execute();

            acceptUseCase.Verify(x => x.Invoke(), Times.Once);
        }

        [Test]
        public void AgreeCommand_WhenAcceptSuccess_ShouldNavigateToMainPage()
        {
            var termsUrl = Faker.Url.Root();
            var privacyPolicyUrl = Faker.Url.Root();
            var navigationService = Mock.NavigationService();
            var getWebPageUrlUseCase = new Mock<IGetWebPageUrlUseCase>();
            var acceptUseCase = new Mock<IAcceptUseCase>();
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var subject = new AcceptPageViewModel(navigationService.Object)
            {
                SchedulerProvider = schedulerProvider,
                GetWebPageUrlUseCase = getWebPageUrlUseCase.Object,
                AcceptUseCase = acceptUseCase.Object
            };

            getWebPageUrlUseCase.Setup(x => x.Invoke(Domain.Models.WebPage.Terms)).ReturnsAsync(termsUrl);
            getWebPageUrlUseCase.Setup(x => x.Invoke(Domain.Models.WebPage.PrivacyPolicy)).ReturnsAsync(privacyPolicyUrl);

            subject.OpenCommand.Execute();

            scheduler.AdvanceBy(1);

            scheduler.AdvanceBy(1);

            subject.LoadedCommand.Execute();

            scheduler.AdvanceBy(1);

            subject.CurrentCommand.Value.Execute();

            scheduler.AdvanceBy(1);

            scheduler.AdvanceBy(1);

            subject.LoadedCommand.Execute();

            scheduler.AdvanceBy(1);

            subject.CurrentCommand.Value.Execute();

            scheduler.AdvanceBy(1);

            navigationService.Verify(x => x.NavigateAsync("/NavigationPage/MainPage", null, null, true), Times.Once());
        }

        [Test]
        public void AgreeCommand_WhenAcceptFail_ShouldDisplayRetryDialog()
        {
            var termsUrl = Faker.Url.Root();
            var privacyPolicyUrl = Faker.Url.Root();
            var navigationService = Mock.NavigationService();
            var pageDialogService = Mock.PageDialogService();
            var getWebPageUrlUseCase = new Mock<IGetWebPageUrlUseCase>();
            var acceptUseCase = new Mock<IAcceptUseCase>();
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var subject = new AcceptPageViewModel(navigationService.Object, pageDialogService.Object)
            {
                SchedulerProvider = schedulerProvider,
                GetWebPageUrlUseCase = getWebPageUrlUseCase.Object,
                AcceptUseCase = acceptUseCase.Object
            };

            getWebPageUrlUseCase.Setup(x => x.Invoke(Domain.Models.WebPage.Terms)).ReturnsAsync(termsUrl);
            getWebPageUrlUseCase.Setup(x => x.Invoke(Domain.Models.WebPage.PrivacyPolicy)).ReturnsAsync(privacyPolicyUrl);

            acceptUseCase.Setup(x => x.Invoke()).Returns(Task.FromException(new Exception()));

            subject.OpenCommand.Execute();

            scheduler.AdvanceBy(1);

            scheduler.AdvanceBy(1);

            subject.LoadedCommand.Execute();

            scheduler.AdvanceBy(1);

            subject.CurrentCommand.Value.Execute();

            scheduler.AdvanceBy(1);

            scheduler.AdvanceBy(1);

            subject.LoadedCommand.Execute();

            scheduler.AdvanceBy(1);

            subject.CurrentCommand.Value.Execute();

            scheduler.AdvanceBy(1);

            navigationService.Verify(x => x.NavigateAsync("/NavigationPage/MainPage", null, null, true), Times.Never());

            pageDialogService.Verify(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        #endregion
    }
}
