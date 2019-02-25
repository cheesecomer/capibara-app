#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using Capibara.Domain.UseCases;
using Moq;
using NUnit.Framework;

namespace Capibara.Presentation.ViewModels
{
    [TestFixture]
    public class AboutPageViewModelTest
    {
        [Test]
        public void Copyright()
        {
            Assert.That(new AboutPageViewModel(Mock.NavigationService().Object).Copyright.Value, Is.EqualTo($"Copyright © 2018-{DateTime.Now.Year} @cheese_comer."));
        }

        [Test]
        public void Version()
        {
            Assert.IsNull(new AboutPageViewModel(Mock.NavigationService().Object).Version.Value);
        }

        [Test]
        public void CloseCommand()
        {
            var navigationService = Mock.NavigationService();
            var subject = new AboutPageViewModel(navigationService.Object);

            subject.CloseCommand.Execute();

            navigationService.Verify(x => x.GoBackAsync(), Times.Once);
        }

        [Test]
        public void OpenCommand()
        {
            var version = Faker.Application.Version();
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var getApplicationVersionUseCase = new Mock<IGetApplicationVersionUseCase>();

            var navigationService = Mock.NavigationService();
            var subject = new AboutPageViewModel(navigationService.Object)
            {
                SchedulerProvider = schedulerProvider,
                GetApplicationVersionUseCase = getApplicationVersionUseCase.Object
            };

            getApplicationVersionUseCase.Setup(x => x.Invoke()).ReturnsAsync(version);

            subject.OpenCommand.Execute();

            getApplicationVersionUseCase.Verify(x => x.Invoke(), Times.Once);

            scheduler.AdvanceBy(1);

            scheduler.AdvanceBy(1);

            Assert.That(subject.Version.Value, Is.EqualTo($"Version {version}"));
        }
    }
}
