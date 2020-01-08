#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Collections;
using System.Threading.Tasks;
using Capibara.Domain.Models;
using Capibara.Domain.UseCases;
using Moq;
using NUnit.Framework;
using Prism.Navigation;

namespace Capibara.Presentation.ViewModels
{
    [TestFixture]
    public class UserProfilePageViewModelTest
    {
        static public IEnumerable Property_TestCaseSource()
        {
            yield return new TestCaseData(
                new Action<User>(x => x.FollowId = null),
                new Func<User, object>(x => false),
                new Func<UserProfilePageViewModel, object>(x => x.IsFollow.Value))
                .SetName("Model.IsFollow Property When False Should IsFollow Property is False");

            yield return new TestCaseData(
                new Action<User>(x => x.FollowId = (x.FollowId ?? Faker.RandomNumber.Next()) + 1),
                new Func<User, object>(x => true),
                new Func<UserProfilePageViewModel, object>(x => x.IsFollow.Value))
                .SetName("Model.IsFollow Property When True Should IsFollow Property is True");

            yield return new TestCaseData(
                new Action<User>(x => x.IsFollower = false),
                new Func<User, object>(x => false),
                new Func<UserProfilePageViewModel, object>(x => x.IsFollower.Value))
                .SetName("Model.IsFollower Property When False Should IsFollower Property is False");

            yield return new TestCaseData(
                new Action<User>(x => x.IsFollower = true),
                new Func<User, object>(x => true),
                new Func<UserProfilePageViewModel, object>(x => x.IsFollower.Value))
                .SetName("Model.IsFollower Property When True Should IsFollower Property is True");

            yield return new TestCaseData(
                new Action<User>(x => x.FollowId = null),
                new Func<User, object>(x => "DM を受け付ける"),
                new Func<UserProfilePageViewModel, object>(x => x.ToggleFollowDescription.Value))
                .SetName("Model.IsFollow Property When False Should ToggleFollowDescription Property is \"DM を受け付ける\"");

            yield return new TestCaseData(
                new Action<User>(x => x.FollowId = (x.FollowId ?? Faker.RandomNumber.Next()) + 1),
                new Func<User, object>(x => "DM を受け付けています"),
                new Func<UserProfilePageViewModel, object>(x => x.ToggleFollowDescription.Value))
                .SetName("Model.IsFollow Property When True Should ToggleFollowDescription Property is \"DM を受け付けています\"");

            yield return new TestCaseData(
                new Action<User>(x => x.BlockId = null),
                new Func<User, object>(x => "ブロック"),
                new Func<UserProfilePageViewModel, object>(x => x.ToggleBlockDescription.Value))
                .SetName("Model.IsBlock Property When False Should ToggleBlockDescription Property is \"ブロック\"");

            yield return new TestCaseData(
                new Action<User>(x => x.BlockId = (x.BlockId ?? Faker.RandomNumber.Next()) + 1),
                new Func<User, object>(x => "ブロック中"),
                new Func<UserProfilePageViewModel, object>(x => x.ToggleBlockDescription.Value))
                .SetName("Model.IsBlock Property When True Should ToggleBlockDescription Property is \"ブロック中\"");
        }

        [Test]
        [TestCaseSource("Property_TestCaseSource")]
        public void Property(Action<User> setter, Func<User, object> expectedGetter, Func<UserProfilePageViewModel, object> getter)
        {
            var subject = new UserProfilePageViewModel(model: ModelFixture.User());
            setter(subject.Model);
            Assert.That(getter(subject), Is.EqualTo(expectedGetter(subject.Model)));
        }

        public static IEnumerable ShowDirectMessageCommand_CanExecute_TestCaseSource()
        {
            yield return
                new TestCaseData(null, false)
                .SetName("ShowDirectMessageCommand When not follow should can not execute");
            yield return
                new TestCaseData(Faker.RandomNumber.Next(), true)
                .SetName("ShowDirectMessageCommand When follow should can execute");
        }

        [Test]
        [TestCaseSource("ShowDirectMessageCommand_CanExecute_TestCaseSource")]
        public void ShowDirectMessageCommand_CanExecute(int? followId, bool expected)
        {
            var subject = new UserProfilePageViewModel(model: ModelFixture.User(followId: followId));
            Assert.That(subject.ShowDirectMessageCommand.CanExecute(), Is.EqualTo(expected));
        }

        public static IEnumerable ToggleFollowCommand_CanExecute_TestCaseSource()
        {
            yield return
                new TestCaseData(null, true)
                .SetName("ToggleFollowCommand When not block should can execute");
            yield return
                new TestCaseData(Faker.RandomNumber.Next(), false)
                .SetName("ToggleFollowCommand When block should can not execute");
        }

        #region ToggleFollowCommand

        [Test]
        [TestCaseSource("ToggleFollowCommand_CanExecute_TestCaseSource")]
        public void ToggleFollowCommand_CanExecute(int? blockId, bool expected)
        {
            var subject = new UserProfilePageViewModel(model: ModelFixture.User(blockId: blockId));
            Assert.That(subject.ToggleFollowCommand.CanExecute(), Is.EqualTo(expected));
        }

        [Test]
        public void ToggleFollowCommand_WhenExecute_ShouldInvokeUseCase()
        {
            var useCase = new Mock<IToggleFollowUseCase>();
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var subject = new UserProfilePageViewModel(model: ModelFixture.User()) { SchedulerProvider = schedulerProvider, ToggleFollowUseCase = useCase.Object };

            subject.ToggleFollowCommand.Execute();

            scheduler.AdvanceBy(1);
            useCase.Verify(x => x.Invoke(It.Is<User>(v => v == subject.Model)), Times.Once);
        }

        [Test]
        public void ToggleFollowCommand_WhenNotComplete_ShouldCannotExecute()
        {
            var useCase = new Mock<IToggleFollowUseCase>();
            var schedulerProvider = new SchedulerProvider();
            var subject = new UserProfilePageViewModel(model: ModelFixture.User()) { SchedulerProvider = schedulerProvider, ToggleFollowUseCase = useCase.Object };

            subject.ToggleFollowCommand.Execute();

            Assert.That(subject.ToggleFollowCommand.CanExecute(), Is.EqualTo(false));
        }

        [Test]
        public void ToggleFollowCommand_WhenError_ShouldShowRetryDialog()
        {
            var useCase = new Mock<IToggleFollowUseCase>();
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var pageDialogService = Mock.PageDialogService();
            var subject = new UserProfilePageViewModel(
                model: ModelFixture.User(),
                pageDialogService: pageDialogService.Object)
            {
                SchedulerProvider = schedulerProvider,
                ToggleFollowUseCase = useCase.Object
            };

            useCase.Setup(x => x.Invoke(It.IsAny<User>())).Returns(Task.FromException(new Exception()));

            subject.ToggleFollowCommand.Execute();

            scheduler.AdvanceBy(1); // Invoke Usecase

            scheduler.AdvanceBy(1); // RetryWhen

            pageDialogService.Verify(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ToggleFollowCommand_WhenRetry_ShouldRetryUseCase()
        {
            var useCase = new Mock<IToggleFollowUseCase>();
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var pageDialogService = Mock.PageDialogService(true);
            var subject = new UserProfilePageViewModel(
                model: ModelFixture.User(),
                pageDialogService: pageDialogService.Object)
            {
                SchedulerProvider = schedulerProvider,
                ToggleFollowUseCase = useCase.Object
            };

            useCase.Setup(x => x.Invoke(It.IsAny<User>())).Returns(Task.FromException(new Exception()));

            subject.ToggleFollowCommand.Execute();

            scheduler.AdvanceBy(1); // Invoke Usecase

            scheduler.AdvanceBy(1); // RetryWhen

            scheduler.AdvanceBy(1); // Invoke Usecase

            useCase.Verify(x => x.Invoke(It.Is<User>(v => v == subject.Model)), Times.Exactly(2));
        }

        #endregion

        #region ToggleBlockCommand

        [Test]
        public void ToggleBlockCommand_WhenExecute_ShouldInvokeUseCase()
        {
            var useCase = new Mock<IToggleBlockUseCase>();
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var subject = new UserProfilePageViewModel(model: ModelFixture.User()) { SchedulerProvider = schedulerProvider, ToggleBlockUseCase = useCase.Object };

            subject.ToggleBlockCommand.Execute();

            scheduler.AdvanceBy(1);
            useCase.Verify(x => x.Invoke(It.Is<User>(v => v == subject.Model)), Times.Once);
        }

        [Test]
        public void ToggleBlockCommand_WhenNotComplete_ShouldCannotExecute()
        {
            var useCase = new Mock<IToggleBlockUseCase>();
            var schedulerProvider = new SchedulerProvider();
            var subject = new UserProfilePageViewModel(model: ModelFixture.User()) { SchedulerProvider = schedulerProvider, ToggleBlockUseCase = useCase.Object };

            subject.ToggleBlockCommand.Execute();

            Assert.That(subject.ToggleBlockCommand.CanExecute(), Is.EqualTo(false));
        }

        [Test]
        public void ToggleBlockCommand_WhenError_ShouldShowRetryDialog()
        {
            var useCase = new Mock<IToggleBlockUseCase>();
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var pageDialogService = Mock.PageDialogService();
            var subject = new UserProfilePageViewModel(
                model: ModelFixture.User(),
                pageDialogService: pageDialogService.Object)
            {
                SchedulerProvider = schedulerProvider,
                ToggleBlockUseCase = useCase.Object
            };

            useCase.Setup(x => x.Invoke(It.IsAny<User>())).Returns(Task.FromException(new Exception()));

            subject.ToggleBlockCommand.Execute();

            scheduler.AdvanceBy(1); // Invoke Usecase

            scheduler.AdvanceBy(1); // RetryWhen

            pageDialogService.Verify(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ToggleBlockCommand_WhenRetry_ShouldRetryUseCase()
        {
            var useCase = new Mock<IToggleBlockUseCase>();
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var pageDialogService = Mock.PageDialogService(true);
            var subject = new UserProfilePageViewModel(
                model: ModelFixture.User(),
                pageDialogService: pageDialogService.Object)
            {
                SchedulerProvider = schedulerProvider,
                ToggleBlockUseCase = useCase.Object
            };

            useCase.Setup(x => x.Invoke(It.IsAny<User>())).Returns(Task.FromException(new Exception()));

            subject.ToggleBlockCommand.Execute();

            scheduler.AdvanceBy(1); // Invoke Usecase

            scheduler.AdvanceBy(1); // RetryWhen

            scheduler.AdvanceBy(1); // Invoke Usecase

            useCase.Verify(x => x.Invoke(It.Is<User>(v => v == subject.Model)), Times.Exactly(2));
        }

        #endregion

        #region ReportCommand

        public void ReportCommand_WhenExecute_ShouldNavigatToReportPage()
        {
            var navigationService = Mock.NavigationService();
            var subject = new UserProfilePageViewModel(
                model: ModelFixture.User(),
                navigationService: navigationService.Object);

            subject.ReportCommand.Execute();

            navigationService
                .Verify(
                    x =>
                        x.NavigateAsync(
                            "ReportPage",
                            It.Is<INavigationParameters>(v => v[ParameterNames.Model] == subject.Model),
                            null,
                            true),
                    Times.Once());
        }

        #endregion
    }
}
