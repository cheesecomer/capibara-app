using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using Prism.Services;

namespace Capibara.Presentation
{
    [TestFixture]
    public class ObservableExtensionsTest
    {
        [Test]
        public void RetryWhen_WhenSuccess_ShouldNotDisplayDialog()
        {
            var scheduler = new TestScheduler();
            var observable = Observable.Return(Unit.Default);
            var observer = scheduler.CreateObserver<Unit>();
            var pageDialogService = new Mock<IPageDialogService>();

            observable
                .RetryWhen(pageDialogService.Object, scheduler)
                .Subscribe(observer);

            pageDialogService.Verify(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void RetryWhen_WhenSuccess_ShouldComplete()
        {
            var scheduler = new TestScheduler();
            var observable = Observable.Return(Unit.Default);
            var observer = scheduler.CreateObserver<Unit>();
            var pageDialogService = new Mock<IPageDialogService>();

            observable
                .RetryWhen(pageDialogService.Object, scheduler)
                .Subscribe(observer);

            scheduler.AdvanceBy(1);

            Assert.That(observer.Messages.Last().Value.Kind, Is.EqualTo(NotificationKind.OnCompleted));
        }

        [Test]
        public void RetryWhen_WhenFailAndNotRetry_ShouldDisplayDialog()
        {
            var scheduler = new TestScheduler();
            var observable = Observable.Throw<Unit>(new Exception());
            var observer = scheduler.CreateObserver<Unit>();
            var pageDialogService = new Mock<IPageDialogService>();

            pageDialogService
                .Setup(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            observable
                .RetryWhen(pageDialogService.Object, scheduler)
                .Subscribe(observer);

            pageDialogService.Verify(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void RetryWhen_WhenFailAndNotRetry_ShouldError()
        {
            var scheduler = new TestScheduler();
            var observable = Observable.Throw<Unit>(new Exception());
            var observer = scheduler.CreateObserver<Unit>();
            var pageDialogService = new Mock<IPageDialogService>();

            pageDialogService
                .Setup(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            observable
                .RetryWhen(pageDialogService.Object, scheduler)
                .Subscribe(observer);

            scheduler.AdvanceBy(1);

            Assert.That(observer.Messages.Last().Value.Kind, Is.EqualTo(NotificationKind.OnError));
        }

        [Test]
        public void RetryWhen_WhenFailAndRetry_ShouldDisplayDialog()
        {
            var scheduler = new TestScheduler();
            var observable = Observable.Throw<Unit>(new Exception());
            var observer = scheduler.CreateObserver<Unit>();
            var pageDialogService = new Mock<IPageDialogService>();

            pageDialogService
                .Setup(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            observable
                .RetryWhen(pageDialogService.Object, scheduler)
                .Subscribe(observer);

            pageDialogService.Verify(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            scheduler.AdvanceBy(1);

            pageDialogService.Verify(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }

        [Test]
        public void RetryWhen_WhenFailAndRetry_ShouldInProggress()
        {
            var scheduler = new TestScheduler();
            var observable = Observable.Throw<Unit>(new Exception());
            var observer = scheduler.CreateObserver<Unit>();
            var pageDialogService = new Mock<IPageDialogService>();

            pageDialogService
                .Setup(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            observable
                .RetryWhen(pageDialogService.Object, scheduler)
                .Subscribe(observer);

            scheduler.AdvanceBy(1);

            scheduler.AdvanceBy(1);

            Assert.IsEmpty(observer.Messages);
        }
    }
}
