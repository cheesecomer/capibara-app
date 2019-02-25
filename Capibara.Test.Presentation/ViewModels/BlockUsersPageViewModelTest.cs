#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
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
    public class BlockUsersPageViewModelTest
    {
        #region When Initialized

        [Test]
        public void Blocks_WhenInitialized_ShouldEmpty()
        {
            Assert.IsEmpty(new BlockUsersPageViewModel().Blocks);
        }

        [Test]
        public void RefreshCommand_WhenInitialized_ShouldCanExecute()
        {
            Assert.True(new BlockUsersPageViewModel().RefreshCommand.CanExecute());
        }

        #endregion

        [Test]
        public void RefreshCommand_WhenExecute_ShouldInvokeFetchBlockUsersUseCase()
        {
            var fetchBlockUsersUseCase = new Mock<IFetchBlockUsersUseCase>();
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var subject = new BlockUsersPageViewModel
            {
                SchedulerProvider = schedulerProvider,
                FetchBlockUsersUseCase = fetchBlockUsersUseCase.Object
            };

            subject.RefreshCommand.Execute();

            fetchBlockUsersUseCase.Verify(x => x.Invoke(), Times.Once);
        }

        #region When RefreshCommand Success

        [Test]
        public void Blocks_WhenRefreshCommandSuccess_ShouldEmpty()
        {
            var fetchBlockUsersUseCase = new Mock<IFetchBlockUsersUseCase>();
            var blocks = Enumerable.Range(0, 10).Select(_ => ModelFixture.Block()).ToList();
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var subject = new BlockUsersPageViewModel
            {
                SchedulerProvider = schedulerProvider,
                FetchBlockUsersUseCase = fetchBlockUsersUseCase.Object
            };

            fetchBlockUsersUseCase.Setup(x => x.Invoke()).ReturnsAsync(blocks);

            subject.RefreshCommand.Execute();

            scheduler.AdvanceBy(1);

            scheduler.AdvanceBy(1);

            Assert.That(subject.Blocks, Is.EqualTo(blocks));
        }

        #endregion
    }
}
