#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Capibara.Domain.Models;
using Capibara.Domain.UseCases;
using Capibara.Presentation.Navigation;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using Prism.Navigation;
using Prism.Services;

namespace Capibara.Presentation.ViewModels
{
    [TestFixture]
    public class FloorMapPageViewModelTest
    {
        #region RefreshCommand

        public class RefreshCommandSubject
        {
            public TestScheduler Scheduler { get; }

            public Mock<NavigationService> NavigationService { get; }

            public Mock<IPageDialogService> PageDialogService { get; }

            public FloorMapPageViewModel ViewModel { get; }

            public Mock<IFetchRoomsUseCase> FetchRoomsUseCase { get; }

            public RefreshCommandSubject(
                ICollection<Room> rooms = null, 
                IEnumerable<Room> currentRooms = null, 
                Exception exception = null, 
                bool shouldRetry = false)
            {
                var schedulerProvider = new SchedulerProvider();

                this.Scheduler = schedulerProvider.Scheduler;
                this.NavigationService = Mock.NavigationService();
                this.PageDialogService = Mock.PageDialogService(shouldRetry);

                this.FetchRoomsUseCase = new Mock<IFetchRoomsUseCase>();
                this.FetchRoomsUseCase
                    .Setup(x => x.Invoke())
                    .Returns(
                        exception != null
                            ? Observable.Throw<ICollection<Room>>(exception)
                            : Observable.Return(rooms ?? new List<Room>()));

                this.ViewModel = new FloorMapPageViewModel(this.NavigationService.Object, this.PageDialogService.Object)
                {
                    SchedulerProvider = schedulerProvider,
                    FetchRoomsUseCase = FetchRoomsUseCase.Object
                };

                if (currentRooms != null)
                {
                    currentRooms.ForEach(x => this.ViewModel.Rooms.Add(x));
                }
            }
        }

        [Test]
        public void RefreshCommand_ShouldInvokeFetchRoomsUseCase()
        {
            var subject = new RefreshCommandSubject();

            subject.ViewModel.RefreshCommand.Execute();

            subject.Scheduler.AdvanceBy(1); // Invoke Usecase

            subject.FetchRoomsUseCase.Verify(x => x.Invoke(), Times.Once);
        }

        [Test]
        public void RefreshCommand_WhenFetchRoomsIsEmpty_ShouldRoomNotChange()
        {
            var subject = new RefreshCommandSubject();

            subject.ViewModel.RefreshCommand.Execute();

            subject.Scheduler.AdvanceBy(1); // Invoke Usecase

            subject.Scheduler.AdvanceBy(1); // Do

            Assert.IsEmpty(subject.ViewModel.Rooms);
        }

        [Test]
        public void RefreshCommand_WhenFetchRoomsIsPresent_ShouldRoomChange()
        {
            var expected = Enumerable.Range(0, 10).Select(_ => ModelFixture.Room()).ToList();
            var subject = new RefreshCommandSubject(expected);

            subject.ViewModel.RefreshCommand.Execute();

            subject.Scheduler.AdvanceBy(1); // Invoke Usecase

            subject.Scheduler.AdvanceBy(1); // Do

            Assert.That(subject.ViewModel.Rooms, Is.EqualTo(expected));
        }

        [Test]
        public void RefreshCommand_WhenFetchRoomsHasExistsRoom_ShouldRoomChange()
        {
            var expected = Enumerable.Range(0, 10).Select(_ => ModelFixture.Room()).ToList();
            var subject = new RefreshCommandSubject(expected, expected.Select(x => ModelFixture.Room(x.Id)).ToList());

            subject.ViewModel.RefreshCommand.Execute();

            subject.Scheduler.AdvanceBy(1); // Invoke Usecase

            subject.Scheduler.AdvanceBy(1); // Do

            Assert.That(subject.ViewModel.Rooms, Is.EqualTo(expected));
        }

        [Test]
        public void RefreshCommand_WhenFailedAndNotRetry_ShouldDisplayAlert()
        {
            var subject = new RefreshCommandSubject(exception: new Exception(), shouldRetry: false);

            subject.ViewModel.RefreshCommand.Execute();

            subject.Scheduler.AdvanceBy(1); // Invoke Usecase

            subject.Scheduler.AdvanceBy(1); // Do

            subject.Scheduler.AdvanceBy(1); // RetryWhen

            subject.PageDialogService.Verify(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void RefreshCommand_WhenFailedAndRetry_ShouldInvokeRefreshSessionUseCaseTwice()
        {
            var subject = new RefreshCommandSubject(exception: new Exception(), shouldRetry: true);
            subject.ViewModel.RefreshCommand.Execute();

            subject.Scheduler.AdvanceBy(1); // Invoke Usecase

            subject.Scheduler.AdvanceBy(1); // Do

            subject.Scheduler.AdvanceBy(1); // RetryWhen

            subject.Scheduler.AdvanceBy(1); // Invoke Usecase

            subject.FetchRoomsUseCase.Verify(x => x.Invoke(), Times.Exactly(2));
        }

        #endregion

        #region ItemTappedCommand

        [Test]
        public void ItemTappedCommand()
        {
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var navigationService = Mock.NavigationService();
            var pageDialogService = Mock.PageDialogService();

            var viewModel = new FloorMapPageViewModel(navigationService.Object, pageDialogService.Object)
            {
                SchedulerProvider = schedulerProvider
            };

            var room = ModelFixture.Room();
            viewModel.ItemTappedCommand.Execute(room);

            navigationService
                .Verify(
                    x => 
                        x.NavigateAsync(
                            "RoomPage", 
                            It.Is<INavigationParameters>(v => v[ParameterNames.Model] == room), 
                            null, 
                            true), 
                    Times.Once());
        }

        #endregion    
    }
}
