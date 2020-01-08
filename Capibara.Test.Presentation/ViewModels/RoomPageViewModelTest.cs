#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Reactive;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Capibara.Domain.Models;
using Capibara.Domain.UseCases;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using Reactive.Bindings.Extensions;
using Moq;

namespace Capibara.Presentation.ViewModels
{
    [TestFixture]
    public class RoomPageViewModelTest
    {
        static private IEnumerable Property_WhenRisePropertyChanged_ShouldUpdate_TestCase()
        {
            yield return new TestCaseData(
                new Func<Room, object>(x => x.Name = Faker.Lorem.Sentence()),
                new Func<RoomPageViewModel, object>(x => x.Name.Value))
                .SetName("Name Property When changed Should change");

            yield return new TestCaseData(
                new Func<Room, object>(x => x.Capacity = x.Capacity + 1),
                new Func<RoomPageViewModel, object>(x => x.Capacity.Value))
                .SetName("Capacity Property When changed Should change");

            yield return new TestCaseData(
                new Func<Room, object>(x => x.NumberOfParticipants = x.NumberOfParticipants + 1),
                new Func<RoomPageViewModel, object>(x => x.NumberOfParticipants.Value))
                .SetName("NumberOfParticipants Property When changed Should change");

            yield return new TestCaseData(
                new Func<Room, object>(x => x.IsConnected = !x.IsConnected),
                new Func<RoomPageViewModel, object>(x => x.IsConnected.Value))
                .SetName("IsConnected Property When changed Should change");
        }

        [Test]
        [TestCaseSource("Property_WhenRisePropertyChanged_ShouldUpdate_TestCase")]
        public void Property_WhenRisePropertyChanged_ShouldUpdate(Func<Room, object> setter, Func<RoomPageViewModel, object> getter)
        {
            var subject = new RoomPageViewModel(model: ModelFixture.Room());
            var expected = setter(subject.Model);
            Assert.That(getter(subject), Is.EqualTo(expected));
        }

        static private IEnumerable PropertyChangedTestCase()
        {
            yield return new TestCaseData(
                new Func<RoomPageViewModel, IObservable<PropertyChangedEventArgs>>(x => x.Name.PropertyChangedAsObservable()),
                new Action<Room>(x => { }), 0)
                .SetName("Name Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                new Func<RoomPageViewModel, IObservable<PropertyChangedEventArgs>>(x => x.Name.PropertyChangedAsObservable()),
                new Action<Room>(x => x.Name = Faker.Lorem.Words(1).First()), 1)
                .SetName("Name Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                new Func<RoomPageViewModel, IObservable<PropertyChangedEventArgs>>(x => x.Capacity.PropertyChangedAsObservable()),
                new Action<Room>(x => { }), 0)
                .SetName("Capacity Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                new Func<RoomPageViewModel, IObservable<PropertyChangedEventArgs>>(x => x.Capacity.PropertyChangedAsObservable()),
                new Action<Room>(x => x.Capacity = x.Capacity + 1), 1)
                .SetName("Capacity Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                new Func<RoomPageViewModel, IObservable<PropertyChangedEventArgs>>(x => x.NumberOfParticipants.PropertyChangedAsObservable()),
                new Action<Room>(x => { }), 0)
                .SetName("NumberOfParticipants Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                new Func<RoomPageViewModel, IObservable<PropertyChangedEventArgs>>(x => x.NumberOfParticipants.PropertyChangedAsObservable()),
                new Action<Room>(x => x.NumberOfParticipants = x.NumberOfParticipants + 1), 1)
                .SetName("NumberOfParticipants Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                new Func<RoomPageViewModel, IObservable<PropertyChangedEventArgs>>(x => x.IsConnected.PropertyChangedAsObservable()),
                new Action<Room>(x => { }), 0)
                .SetName("Sender Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                new Func<RoomPageViewModel, IObservable<PropertyChangedEventArgs>>(x => x.IsConnected.PropertyChangedAsObservable()),
                new Action<Room>(x => x.IsConnected = !x.IsConnected), 1)
                .SetName("Sender Property When chnaged Should raise PropertyChanged");
        }

        [Test]
        [TestCaseSource("PropertyChangedTestCase")]
        public void PropertyChanged(
            Func<RoomPageViewModel, IObservable<PropertyChangedEventArgs>> observableGetter,
            Action<Room> setter,
            int expected)
        {
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<PropertyChangedEventArgs>();
            var subject = new RoomPageViewModel(model: ModelFixture.Room());

            observableGetter(subject)
                .Where(x => x.PropertyName == "Value")
                .Subscribe(observer);

            setter(subject.Model);

            Assert.That(observer.Messages.Count, Is.EqualTo(expected));
        }

        #region SpeekCommandCanExecute

        [TestCase(false, "", false, "", false, TestName = "SpeekCommand When Not Connected And Message Is Empty Should Can not Execute")]
        [TestCase(false, "", true, "", false, TestName = "SpeekCommand When From Not Connected to Conneted And Message Is Empty Should Can not Execute")]
        [TestCase(true, "", false, "", false, TestName = "SpeekCommand When From Connected to Not Conneted And Message Is Empty Should Can not Execute")]
        [TestCase(true, "", true, "", false, TestName = "SpeekCommand When Connected And Message Is Empty Should Can not Execute")]
        [TestCase(false, null, false, "", false, TestName = "SpeekCommand When Not Connected And Message From Null To Empty Should Can not Execute")]
        [TestCase(false, null, true, "", false, TestName = "SpeekCommand When From Not Connected to Conneted And Message From Null To Empty Should Can not Execute")]
        [TestCase(true, null, false, "", false, TestName = "SpeekCommand When From Connected to Not Conneted And Message From Null To Empty Should Can not Execute")]
        [TestCase(true, null, true, "", false, TestName = "SpeekCommand When Connected And Message From Null To Empty Should Can not Execute")]
        [TestCase(false, null, false, null, false, TestName = "SpeekCommand When Not Connected And Message Is Null Should Can not Execute")]
        [TestCase(false, null, true, null, false, TestName = "SpeekCommand When From Not Connected to Conneted And Message Is Null Should Can not Execute")]
        [TestCase(true, null, false, null, false, TestName = "SpeekCommand When From Connected to Not Conneted And Message Is Null Should Can not Execute")]
        [TestCase(true, null, true, null, false, TestName = "SpeekCommand When Connected And Message Is Null Should Can not Execute")]
        [TestCase(false, "", false, "Foo", false, TestName = "SpeekCommand When Not Connected And Message From Empty To Present Should Can not Execute")]
        [TestCase(false, "", true, "Foo", true, TestName = "SpeekCommand When From Not Connected To Connected And Message From Empty To Present Should Can Execute")]
        [TestCase(true, "", false, "Foo", false, TestName = "SpeekCommand When From Connected To Not Connected And Message From Empty To Present Should Can not Execute")]
        [TestCase(true, "", true, "Foo", true, TestName = "SpeekCommand When Connected And Message From Empty To Present Should Can Execute")]
        [TestCase(false, null, false, "Foo", false, TestName = "SpeekCommand When Not Connected And Message From Null To Present Should Can not Execute")]
        [TestCase(false, null, true, "Foo", true, TestName = "SpeekCommand When From Not Connected To Connected And Message From Null To Present Should Can Execute")]
        [TestCase(true, null, false, "Foo", false, TestName = "SpeekCommand When From Connected To Not Connected And Message From Null To Present Should Can not Execute")]
        [TestCase(true, null, true, "Foo", true, TestName = "SpeekCommand When Connected And Message From Null To Present Should Can Execute")]
        [TestCase(false, "Foo", false, "Foo", false, TestName = "SpeekCommand When Not Connected And Message Is Present Should Can not Execute")]
        [TestCase(false, "Foo", true, "Foo", true, TestName = "SpeekCommand When From Not Connected To Connected And Message Is Present Should Can Execute")]
        [TestCase(true, "Foo", false, "Foo", false, TestName = "SpeekCommand When From Connected To Not Connected And Message Is Present Should Can not Execute")]
        [TestCase(true, "Foo", true, "Foo", true, TestName = "SpeekCommand When Connected And Message Is Present Should Can Execute")]
        [TestCase(false, " ", false, " ", false, TestName = "SpeekCommand When Not Connected And Message Is Blank Should Can not Execute")]
        [TestCase(false, " ", true, " ", false, TestName = "SpeekCommand When From Not Connected to Conneted And Message Is Blank Should Can not Execute")]
        [TestCase(true, " ", false, " ", false, TestName = "SpeekCommand When From Connected to Not Conneted And Message Is Blank Should Can not Execute")]
        [TestCase(true, " ", true, " ", false, TestName = "SpeekCommand When Connected And Message Is Blank Should Can not Execute")]
        [TestCase(false, "\r", false, "\r", false, TestName = "SpeekCommand When Not Connected And Message Is CR Should Can not Execute")]
        [TestCase(false, "\r", true, "\r", false, TestName = "SpeekCommand When From Not Connected to Conneted And Message Is CR Should Can not Execute")]
        [TestCase(true, "\r", false, "\r", false, TestName = "SpeekCommand When From Connected to Not Conneted And Message Is CR Should Can not Execute")]
        [TestCase(true, "\r", true, "\r", false, TestName = "SpeekCommand When Connected And Message Is CR Should Can not Execute")]
        [TestCase(false, "\n", false, "\n", false, TestName = "SpeekCommand When Not Connected And Message Is LF Should Can not Execute")]
        [TestCase(false, "\n", true, "\n", false, TestName = "SpeekCommand When From Not Connected to Conneted And Message Is LF Should Can not Execute")]
        [TestCase(true, "\n", false, "\n", false, TestName = "SpeekCommand When From Connected to Not Conneted And Message Is LF Should Can not Execute")]
        [TestCase(true, "\n", true, "\n", false, TestName = "SpeekCommand When Connected And Message Is LF Should Can not Execute")]
        [TestCase(false, "\r\n", false, "\r\n", false, TestName = "SpeekCommand When Not Connected And Message Is CRLF Should Can not Execute")]
        [TestCase(false, "\r\n", true, "\r\n", false, TestName = "SpeekCommand When From Not Connected to Conneted And Message Is CRLF Should Can not Execute")]
        [TestCase(true, "\r\n", false, "\r\n", false, TestName = "SpeekCommand When From Connected to Not Conneted And Message Is CRLF Should Can not Execute")]
        [TestCase(true, "\r\n", true, "\r\n", false, TestName = "SpeekCommand When Connected And Message Is CRLF Should Can not Execute")]
        [TestCase(false, " ", false, "Foo. ", false, TestName = "SpeekCommand When Not Connected And Message From Blank To Present Should Can not Execute")]
        [TestCase(false, " ", true, "Foo. ", true, TestName = "SpeekCommand When From Not Connected to Conneted And Message From Blank To Present Should Can Execute")]
        [TestCase(true, " ", false, "Foo. ", false, TestName = "SpeekCommand When From Connected to Not Conneted And Message From Blank To Present Should Can not Execute")]
        [TestCase(true, " ", true, "Foo. ", true, TestName = "SpeekCommand When Connected And Message From Blank To Present Should Can Execute")]
        [TestCase(false, "\r", false, "Foo.\r", false, TestName = "SpeekCommand When Not Connected And Message From CR To Present Should Can not Execute")]
        [TestCase(false, "\r", true, "Foo.\r", true, TestName = "SpeekCommand When From Not Connected to Conneted And Message From CR To Present Should Can not Execute")]
        [TestCase(true, "\r", false, "Foo.\r", false, TestName = "SpeekCommand When From Connected to Not Conneted And Message From CR To Present Should Can Execute")]
        [TestCase(true, "\r", true, "Foo.\r", true, TestName = "SpeekCommand When Connected And Message From CR To Present Should Can not Execute")]
        [TestCase(false, "\n", false, "Foo.\n", false, TestName = "SpeekCommand When Not Connected And Message From LF To Present Should Can Execute")]
        [TestCase(false, "\n", true, "Foo.\n", true, TestName = "SpeekCommand When From Not Connected to Conneted And Message From LF To Present Should Can Execute")]
        [TestCase(true, "\n", false, "Foo.\n", false, TestName = "SpeekCommand When From Connected to Not Conneted And Message From LF To Present Should Can not Execute")]
        [TestCase(true, "\n", true, "Foo.\n", true, TestName = "SpeekCommand When Connected And Message From LF To Present Should Can Execute")]
        [TestCase(false, "\r\n", false, "Foo.\r\n", false, TestName = "SpeekCommand When Not Connected And Message From CRLF To Present Should Can not Execute")]
        [TestCase(false, "\r\n", true, "Foo.\r\n", true, TestName = "SpeekCommand When From Not Connected to Conneted And Message From CRLF To Present Should Can Execute")]
        [TestCase(true, "\r\n", false, "Foo.\r\n", false, TestName = "SpeekCommand When From Connected to Not Conneted And Message From CRLF To Present Should Can not Execute")]
        [TestCase(true, "\r\n", true, "Foo.\r\n", true, TestName = "SpeekCommand When Connected And Message From CRLF To Present Should Can Execute")]
        public void SpeekCommandCanExecute(
            bool beforeConnected, 
            string beforeMessage, 
            bool afterConnected,
            string afterMessage, 
            bool expected)
        {
            var subject = new RoomPageViewModel();
            subject.Model.IsConnected = beforeConnected;
            subject.Message.Value = beforeMessage;

            if (afterConnected != beforeConnected)
                subject.Model.IsConnected = afterConnected;

            if (afterMessage != beforeMessage)
                subject.Message.Value = afterMessage;

            Assert.That(subject.SpeekCommand.CanExecute(), Is.EqualTo(expected));
        }

        #endregion

        #region SpeekCommand

        [Test]
        public void SpeekCommand_WhenSuccess_ShouldInvokeSpeekUseCase()
        {
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var speekUseCase = new Mock<ISpeekUseCase>();
            var message = Faker.Lorem.Sentence();
            var subject = new RoomPageViewModel
            {
                SpeekUseCase = speekUseCase.Object,
                SchedulerProvider = schedulerProvider
            };

            speekUseCase.Setup(x => x.Invoke(It.IsAny<string>())).ReturnsObservable(Unit.Default);

            subject.Model.IsConnected = true;
            subject.Message.Value = message;
            subject.SpeekCommand.Execute();

            scheduler.AdvanceBy(1); // Invoke UseCase
            speekUseCase.Verify(x => x.Invoke(message), Times.Once);
        }

        [Test]
        public void SpeekCommand_WhenSuccess_ShouldMessageClear()
        {
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var speekUseCase = new Mock<ISpeekUseCase>();
            var message = Faker.Lorem.Sentence();
            var subject = new RoomPageViewModel
            {
                SpeekUseCase = speekUseCase.Object,
                SchedulerProvider = schedulerProvider
            };

            speekUseCase.Setup(x => x.Invoke(It.IsAny<string>())).ReturnsObservable(Unit.Default);

            subject.Model.IsConnected = true;
            subject.Message.Value = message;
            subject.SpeekCommand.Execute();

            scheduler.AdvanceBy(1); // Invoke UseCase
            scheduler.AdvanceBy(1); // ObserveOn
            scheduler.AdvanceBy(1); // Do

            Assert.IsEmpty(subject.Message.Value);
        }

        [Test]
        public void SpeekCommand_WhenFail()
        {
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var speekUseCase = new Mock<ISpeekUseCase>();
            var pageDialogService = Mock.PageDialogService();
            var message = Faker.Lorem.Sentence();
            var subject = new RoomPageViewModel(pageDialogService: pageDialogService.Object)
            {
                SpeekUseCase = speekUseCase.Object,
                SchedulerProvider = schedulerProvider
            };

            pageDialogService
                .Setup(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            speekUseCase.Setup(x => x.Invoke(It.IsNotNull<string>())).ReturnsObservable(new Exception());

            subject.Model.IsConnected = true;
            subject.Message.Value = message;
            subject.SpeekCommand.Execute();

            scheduler.AdvanceBy(1); // Invoke UseCase
            speekUseCase.Verify(x => x.Invoke(message), Times.Once);

            scheduler.AdvanceBy(1); // RetryWhen
            scheduler.AdvanceBy(1); // ObserveOn
            pageDialogService.Verify(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            scheduler.AdvanceBy(1);
            Assert.That(subject.Message.Value, Is.EqualTo(message));
        }

        [Test]
        public void SpeekCommand_WhenFailAndShouldRetry()
        {
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var speekUseCase = new Mock<ISpeekUseCase>();
            var pageDialogService = Mock.PageDialogService();
            var message = Faker.Lorem.Sentence();
            var subject = new RoomPageViewModel(pageDialogService: pageDialogService.Object)
            {
                SpeekUseCase = speekUseCase.Object,
                SchedulerProvider = schedulerProvider
            };

            pageDialogService
                .Setup(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            speekUseCase.Setup(x => x.Invoke(It.IsNotNull<string>())).ReturnsObservable(new Exception());

            subject.Model.IsConnected = true;
            subject.Message.Value = message;
            subject.SpeekCommand.Execute();

            scheduler.AdvanceBy(1);
            speekUseCase.Verify(x => x.Invoke(message), Times.Once);

            scheduler.AdvanceBy(1);
            scheduler.AdvanceBy(1);
            pageDialogService.Verify(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            scheduler.AdvanceBy(1);
            speekUseCase.Verify(x => x.Invoke(message), Times.Exactly(2));

            Assert.That(subject.Message.Value, Is.EqualTo(message));
        }

        #endregion

        #region SpeekCommandCanExecute

        [TestCase(false, TestName = "AttachmentImageCommand When Not Connected Can not Execute")]
        [TestCase(true, TestName = "AttachmentImageCommand When Connected Should Can Execute")]
        public void AttachmentImageCommandCanExecute(bool isConnected)
        {
            var subject = new RoomPageViewModel();
            subject.Model.IsConnected = isConnected;
            Assert.That(subject.AttachmentImageCommand.CanExecute(), Is.EqualTo(isConnected));
        }

        #endregion

        #region AttachmentImageCommand

        [Test]
        public void AttachmentImageCommand_WhenSuccess()
        {
            var imageBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8/5+hHgAHggJ/PchI7wAAAABJRU5ErkJggg==";
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var pickupPhotoFromAlbumUseCase = new Mock<IPickupPhotoFromAlbumUseCase>();
            var attachmentImageUseCase = new Mock<IAttachmentImageUseCase>();
            var subject = new RoomPageViewModel
            {
                AttachmentImageUseCase = attachmentImageUseCase.Object,
                PickupPhotoFromAlbumUseCase = pickupPhotoFromAlbumUseCase.Object,
                SchedulerProvider = schedulerProvider
            };

            pickupPhotoFromAlbumUseCase.Setup(x => x.Invoke()).ReturnsObservable(imageBase64);
            attachmentImageUseCase.Setup(x => x.Invoke(It.IsAny<string>())).ReturnsObservable(Unit.Default);

            subject.Model.IsConnected = true;
            subject.AttachmentImageCommand.Execute();

            scheduler.AdvanceBy(1); // Invoke UseCase
            pickupPhotoFromAlbumUseCase.Verify(x => x.Invoke(), Times.Once);

            scheduler.AdvanceBy(1); // Invoke UseCase
            attachmentImageUseCase.Verify(x => x.Invoke(imageBase64), Times.Once);
        }

        [Test]
        public void AttachmentImageCommand_WhenPickupPhotoFromAlbumUseCaseFail()
        {
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var pickupPhotoFromAlbumUseCase = new Mock<IPickupPhotoFromAlbumUseCase>();
            var attachmentImageUseCase = new Mock<IAttachmentImageUseCase>();
            var subject = new RoomPageViewModel
            {
                AttachmentImageUseCase = attachmentImageUseCase.Object,
                PickupPhotoFromAlbumUseCase = pickupPhotoFromAlbumUseCase.Object,
                SchedulerProvider = schedulerProvider
            };

            pickupPhotoFromAlbumUseCase.Setup(x => x.Invoke()).ReturnsObservable(new Exception());

            subject.Model.IsConnected = true;
            subject.AttachmentImageCommand.Execute();

            scheduler.AdvanceBy(1); // Invoke UseCase
            pickupPhotoFromAlbumUseCase.Verify(x => x.Invoke(), Times.Once);

            scheduler.AdvanceBy(1); // Invoke UseCase
            attachmentImageUseCase.Verify(x => x.Invoke(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void AttachmentImageCommand_WhenAttachmentImageUseCaseFail()
        {
            var imageBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8/5+hHgAHggJ/PchI7wAAAABJRU5ErkJggg==";
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var pageDialogService = Mock.PageDialogService();
            var pickupPhotoFromAlbumUseCase = new Mock<IPickupPhotoFromAlbumUseCase>();
            var attachmentImageUseCase = new Mock<IAttachmentImageUseCase>();
            var subject = new RoomPageViewModel(pageDialogService: pageDialogService.Object)
            {
                AttachmentImageUseCase = attachmentImageUseCase.Object,
                PickupPhotoFromAlbumUseCase = pickupPhotoFromAlbumUseCase.Object,
                SchedulerProvider = schedulerProvider
            };

            pickupPhotoFromAlbumUseCase.Setup(x => x.Invoke()).ReturnsObservable(imageBase64);
            attachmentImageUseCase.Setup(x => x.Invoke(It.IsAny<string>())).ReturnsObservable(new Exception());

            pageDialogService
                .Setup(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            subject.Model.IsConnected = true;
            subject.AttachmentImageCommand.Execute();

            scheduler.AdvanceBy(1); // Invoke UseCase
            pickupPhotoFromAlbumUseCase.Verify(x => x.Invoke(), Times.Once);

            scheduler.AdvanceBy(1); // Invoke UseCase
            attachmentImageUseCase.Verify(x => x.Invoke(It.IsAny<string>()), Times.Once);

            scheduler.AdvanceBy(1);
            scheduler.AdvanceBy(1);
            pageDialogService.Verify(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void AttachmentImageCommand_WhenAttachmentImageUseCaseFailAndShouldRetry()
        {
            var imageBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8/5+hHgAHggJ/PchI7wAAAABJRU5ErkJggg==";
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var pageDialogService = Mock.PageDialogService();
            var pickupPhotoFromAlbumUseCase = new Mock<IPickupPhotoFromAlbumUseCase>();
            var attachmentImageUseCase = new Mock<IAttachmentImageUseCase>();
            var subject = new RoomPageViewModel(pageDialogService: pageDialogService.Object)
            {
                AttachmentImageUseCase = attachmentImageUseCase.Object,
                PickupPhotoFromAlbumUseCase = pickupPhotoFromAlbumUseCase.Object,
                SchedulerProvider = schedulerProvider
            };

            pickupPhotoFromAlbumUseCase.Setup(x => x.Invoke()).ReturnsObservable(imageBase64);
            attachmentImageUseCase.Setup(x => x.Invoke(It.IsAny<string>())).ReturnsObservable(new Exception());

            pageDialogService
                .Setup(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            subject.Model.IsConnected = true;
            subject.AttachmentImageCommand.Execute();

            scheduler.AdvanceBy(1); // Invoke UseCase
            pickupPhotoFromAlbumUseCase.Verify(x => x.Invoke(), Times.Once);

            scheduler.AdvanceBy(1); // Invoke UseCase
            attachmentImageUseCase.Verify(x => x.Invoke(It.IsAny<string>()), Times.Once);

            scheduler.AdvanceBy(1);
            scheduler.AdvanceBy(1);
            pageDialogService.Verify(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            scheduler.AdvanceBy(1);
            pickupPhotoFromAlbumUseCase.Verify(x => x.Invoke(), Times.Once);
            attachmentImageUseCase.Verify(x => x.Invoke(It.IsAny<string>()), Times.Exactly(2));
        }

        #endregion
    }
}
