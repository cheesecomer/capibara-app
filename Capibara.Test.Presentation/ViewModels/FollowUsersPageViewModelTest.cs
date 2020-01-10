#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Capibara.Services;
using Capibara.Domain.Models;
using Capibara.Domain.UseCases;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using Prism.Navigation;
using Prism.Services;

namespace Capibara.Presentation.ViewModels
{
    [TestFixture]
    public class FollowUsersPageViewModelTest
    {
        #region RefreshCommand

        public class RefreshCommandTestSubject
        {
            public TestScheduler Scheduler { get; private set; }

            public FollowUsersPageViewModel ViewModel { get; private set; }

            public Mock<IFetchFollowUsersUseCase> FetchFollowUsersUseCase { get; private set; }

            public Mock<IPageDialogService> PageDialogService { get; private set; }

            public Mock<IProgressDialogService> ProgressDialogService { get; private set; }

            public Mock<NavigationService> NavigationService { get; private set; }

            public ICollection<User> FollowUsers { get; set; }

            public RefreshCommandTestSubject()
            {
                var schedulerProvider = new SchedulerProvider();
                var scheduler = schedulerProvider.Scheduler;
                var navigationService = Mock.NavigationService();
                var pageDialogService = Mock.PageDialogService(null);
                var progressDialogService = Mock.ProgressDialogService<ICollection<User>>();
                var useCase = new Mock<IFetchFollowUsersUseCase>();
                var followUsers = ModelFixture.UserCollection(size: 10);
                var viewModel = new FollowUsersPageViewModel(navigationService.Object, pageDialogService.Object)
                {
                    ProgressDialogService = progressDialogService.Object,
                    SchedulerProvider = schedulerProvider,
                    FetchFollowUsersUseCase = useCase.Object,
                };

                this.FollowUsers = followUsers;
                this.Scheduler = scheduler;
                this.ViewModel = viewModel;
                this.FetchFollowUsersUseCase = useCase;
                this.PageDialogService = pageDialogService;
                this.ProgressDialogService = progressDialogService;
                this.NavigationService = navigationService;

                this.FetchFollowUsersUseCase.Setup(x => x.Invoke()).ReturnsObservable(this.FollowUsers);
            }
        }

        public static IEnumerable RefreshCommand_WhenSuccess_TestCaseSource()
        {
            yield return
                new TestCaseData(
                    0,
                    new Action<RefreshCommandTestSubject>(subject => Assert.IsEmpty(subject.ViewModel.FollowUsers)))
                .SetName("RefreshCommand when before execute should collection empty");

            yield return
                new TestCaseData(
                    1,
                    new Action<RefreshCommandTestSubject>(subject => subject.FetchFollowUsersUseCase.Verify(x => x.Invoke())))
                .SetName("RefreshCommand when execute should invoke usecase");

            yield return
                new TestCaseData(
                    1,
                    new Action<RefreshCommandTestSubject>(subject => Assert.That(subject.ViewModel.FollowUsers, Is.EqualTo(subject.FollowUsers))))
                .SetName("RefreshCommand when success should update collection");
        }

        [Test]
        [TestCaseSource("RefreshCommand_WhenSuccess_TestCaseSource")]
        public void RefreshCommand_WhenSuccess(int advanceTime, Action<RefreshCommandTestSubject> assert)
        {
            var subject = new RefreshCommandTestSubject();

            subject.ViewModel.RefreshCommand.Execute();

            subject.Scheduler.AdvanceBy(advanceTime);

            assert(subject);
        }

        #endregion

        #region ItemTappedCommand

        [Test]
        public void ItemTappedCommandTest()
        {
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var navigationService = Mock.NavigationService();
            var pageDialogService = Mock.PageDialogService(null);
            var progressDialogService = Mock.ProgressDialogService<ICollection<User>>();
            var useCase = new Mock<IFetchFollowUsersUseCase>();
            var followUsers = ModelFixture.UserCollection(size: 10);
            var model = ModelFixture.User();
            var viewModel = new FollowUsersPageViewModel(navigationService.Object, pageDialogService.Object)
            {
                ProgressDialogService = progressDialogService.Object,
                SchedulerProvider = schedulerProvider,
                FetchFollowUsersUseCase = useCase.Object,
            };

            viewModel.ItemTappedCommand.Execute(model);

            navigationService
                .Verify(
                    x =>
                        x.NavigateAsync(
                            "UserProfilePage",
                            It.Is<INavigationParameters>(v => v[ParameterNames.Model] == model),
                            null,
                            true),
                    Times.Once());
        }

        #endregion
    }
}
