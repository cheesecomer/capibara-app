using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive;

using Capibara.Domain.Models;
using Capibara.Domain.UseCases;
using Capibara.Presentation.Navigation;
using Capibara.Services;

using Moq;
using NUnit.Framework;
using Prism.Navigation;
using Prism.Services;
using Microsoft.Reactive.Testing;

namespace Capibara.Presentation.ViewModels
{
    public class ParticipantsPageViewModelTest
    {
        #region RefreshCommand

        public class RefreshCommandSubject
        {
            public TestScheduler Scheduler { get; }

            public Room Model { get; }

            public ParticipantsPageViewModel ViewModel { get; }

            public Mock<IFetchRoomParticipantsUseCase> FetchRoomParticipantsUseCase { get; }

            public Mock<IPageDialogService> PageDialogService { get; }

            public Mock<IProgressDialogService> ProgressDialogService { get; }

            public Mock<NavigationService> NavigationService { get; }

            public RefreshCommandSubject(bool? alertResult = null)
            {
                var schedulerProvider = new SchedulerProvider();
                var scheduler = schedulerProvider.Scheduler;
                var navigationService = Mock.NavigationService();
                var pageDialogService = Mock.PageDialogService(alertResult);
                var progressDialogService = Mock.ProgressDialogService<ICollection<User>>();
                var useCase = new Mock<IFetchRoomParticipantsUseCase>();

                var model = ModelFixture.Room();
                var viewModel = new ParticipantsPageViewModel(
                    model: model,
                    navigationService: navigationService.Object,
                    pageDialogService: pageDialogService.Object)
                {
                    FetchRoomParticipantsUseCase = useCase.Object,
                    SchedulerProvider = schedulerProvider,
                    ProgressDialogService = progressDialogService.Object
                };

                this.Scheduler = scheduler;
                this.Model = model;
                this.ViewModel = viewModel;
                this.FetchRoomParticipantsUseCase = useCase;
                this.PageDialogService = pageDialogService;
                this.ProgressDialogService = progressDialogService;
                this.NavigationService = navigationService;
            }
        }

        public static IEnumerable RefreshCommand_TestCaseSource()
        {
            yield return
                new TestCaseData(
                    1,
                    new Action<RefreshCommandSubject>(subject =>
                    subject.FetchRoomParticipantsUseCase
                        .Verify(x => x.Invoke(subject.Model), Times.Once)))
                .SetName("RefreshCommand when execute should invoke usecase");
        }

        [Test]
        [TestCaseSource("RefreshCommand_TestCaseSource")]
        public void RefreshCommand(int advanceTime, Action<RefreshCommandSubject> assert)
        {
            var subject = new RefreshCommandSubject();

            subject.FetchRoomParticipantsUseCase
                .Setup(x => x.Invoke(It.IsAny<Room>()))
                .ReturnsObservable(ModelFixture.UserCollection());

            subject.ViewModel.RefreshCommand.Execute();

            subject.Scheduler.AdvanceBy(advanceTime);

            assert(subject);
        }

        public static IEnumerable RefreshCommand_WhenFail_TestCaseSource()
        {
            yield return
                new TestCaseData(
                    1,
                    new Action<RefreshCommandSubject>(subject =>
                    subject.FetchRoomParticipantsUseCase
                        .Verify(x => x.Invoke(subject.Model), Times.Once)))
                .SetName("RefreshCommand when execute should invoke usecase");

            yield return
                new TestCaseData(
                    3,
                    new Action<RefreshCommandSubject>(subject =>
                        subject.PageDialogService.Verify(x =>
                            x.DisplayAlertAsync(
                                "申し訳ございません！",
                                "通信エラーです。リトライしますか？。",
                                "リトライ",
                                "閉じる"), Times.Once)))
                .SetName("RefreshCommand when usecase fail should show retry dialog");
        }

        [Test]
        [TestCaseSource("RefreshCommand_WhenFail_TestCaseSource")]
        public void RefreshCommand_WhenFail(int advanceTime, Action<RefreshCommandSubject> assert)
        {
            var subject = new RefreshCommandSubject();

            subject.FetchRoomParticipantsUseCase
                .Setup(x => x.Invoke(It.IsAny<Room>()))
                .ReturnsObservable(new Exception());

            subject.ViewModel.RefreshCommand.Execute();

            subject.Scheduler.AdvanceBy(advanceTime);

            assert(subject);
        }

        #endregion

        #region ItemTappedCommand

        public class ItemTappedCommandSubject
        {
            public TestScheduler Scheduler { get; }

            public Room Model { get; }

            public User CurrentUser { get; }

            public User OtherUser { get; }

            public ParticipantsPageViewModel ViewModel { get; }

            public Mock<IGetCurrentUserUseCase> GetCurrentUserUseCase { get; }

            public Mock<IPageDialogService> PageDialogService { get; }

            public Mock<IProgressDialogService> ProgressDialogService { get; }

            public Mock<NavigationService> NavigationService { get; }

            public ItemTappedCommandSubject(bool? alertResult = null)
            {
                var schedulerProvider = new SchedulerProvider();
                var scheduler = schedulerProvider.Scheduler;
                var navigationService = Mock.NavigationService();
                var pageDialogService = Mock.PageDialogService(alertResult);
                var progressDialogService = Mock.ProgressDialogService<ICollection<User>>();
                var useCase = new Mock<IGetCurrentUserUseCase>();

                var model = ModelFixture.Room();
                var viewModel = new ParticipantsPageViewModel(
                    model: model,
                    navigationService: navigationService.Object,
                    pageDialogService: pageDialogService.Object)
                {
                    GetCurrentUserUseCase = useCase.Object,
                    SchedulerProvider = schedulerProvider,
                    ProgressDialogService = progressDialogService.Object
                };

                this.Scheduler = scheduler;
                this.Model = model;
                this.ViewModel = viewModel;
                this.GetCurrentUserUseCase = useCase;
                this.PageDialogService = pageDialogService;
                this.ProgressDialogService = progressDialogService;
                this.NavigationService = navigationService;
                this.CurrentUser = ModelFixture.User();
                this.OtherUser = ModelFixture.User();

                this.GetCurrentUserUseCase.Setup(x => x.Invoke()).ReturnsObservable(this.CurrentUser);
            }
        }

        public static IEnumerable ItemTappedCommandTestCaseSource()
        {
            yield return
                new TestCaseData(
                    2,
                    new Func<ItemTappedCommandSubject, User>(x => x.CurrentUser),
                    new Action<ItemTappedCommandSubject>(subject => subject.GetCurrentUserUseCase.Verify(x => x.Invoke(), Times.Once))
                    )
                .SetName("ItemTappedCommand should invoke usecase");

            yield return
                new TestCaseData(
                    3,
                    new Func<ItemTappedCommandSubject, User>(x => x.CurrentUser),
                    new Action<ItemTappedCommandSubject>(subject =>
                        subject.NavigationService
                            .Verify(
                                x =>
                                    x.NavigateAsync(
                                        "MyProfilePage",
                                        It.Is<NavigationParameters>(parameters =>
                                                parameters.Count == 1
                                            && parameters[ParameterNames.Model] == subject.CurrentUser),
                                        null,
                                        true),
                                Times.Once())))
                .SetName("ItemTappedCommand when current user should navigate to MyProfilePage");

            yield return
                new TestCaseData(
                    3,
                    new Func<ItemTappedCommandSubject, User>(x => x.OtherUser),
                    new Action<ItemTappedCommandSubject>(subject =>
                        subject.NavigationService
                            .Verify(
                                x =>
                                    x.NavigateAsync(
                                        "UserProfilePage",
                                        It.Is<NavigationParameters>(parameters =>
                                                parameters.Count == 1
                                            && parameters[ParameterNames.Model] == subject.OtherUser),
                                        null,
                                        true),
                                Times.Once())))
                .SetName("ItemTappedCommand when other user should navigate to UserProfilePage");
        }

        [Test]
        [TestCaseSource("ItemTappedCommandTestCaseSource")]
        public void ItemTappedCommand(
            int advanceTime,
            Func<ItemTappedCommandSubject, User> parameterSelector,
            Action<ItemTappedCommandSubject> assert)
        {
            var subject = new ItemTappedCommandSubject();

            subject.ViewModel.ItemTappedCommand.Execute(parameterSelector(subject));

            subject.Scheduler.AdvanceBy(advanceTime);

            assert(subject);
        }

        #endregion
    }
}
