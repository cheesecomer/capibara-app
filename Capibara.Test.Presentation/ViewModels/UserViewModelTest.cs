#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using Reactive.Bindings.Extensions;
using Moq;
using Capibara.Domain.Models;
using Capibara.Domain.UseCases;

namespace Capibara.Presentation.ViewModels
{
    [TestFixture]
    public class UserViewModelTest
    {
        static private IEnumerable Property_WhenRisePropertyChanged_ShouldUpdate_TestCase()
        {
            yield return new TestCaseData(
                new Func<User, object>(x => x.Nickname = Faker.Name.FullName()),
                new Func<UserViewModel, object>(x => x.Nickname.Value))
                .SetName("Nickname Property When changed Should change");

            yield return new TestCaseData(
                new Func<User, object>(x => x.Biography = Faker.Lorem.Paragraph()),
                new Func<UserViewModel, object>(x => x.Biography.Value))
                .SetName("Biography Property When changed Should change");

            yield return new TestCaseData(
                new Func<User, object>(x => x.IconUrl = Faker.Url.Image()),
                new Func<UserViewModel, object>(x => x.IconUrl.Value))
                .SetName("IconUrl Property When changed Should change");

            yield return new TestCaseData(
                new Func<User, object>(x => x.IconThumbnailUrl = Faker.Url.Image()),
                new Func<UserViewModel, object>(x => x.IconThumbnailUrl.Value))
                .SetName("IconThumbnailUrl Property When changed Should change");
        }

        [Test]
        [TestCaseSource("Property_WhenRisePropertyChanged_ShouldUpdate_TestCase")]
        public void Property_WhenRisePropertyChanged_ShouldUpdate(Func<User, object> setter, Func<UserViewModel, object> getter)
        {
            var subject = new UserViewModel(model: ModelFixture.User());
            var expected = setter(subject.Model);
            Assert.That(getter(subject), Is.EqualTo(expected));
        }

        static private IEnumerable PropertyChangedTestCase()
        {
            yield return new TestCaseData(
                new Func<UserViewModel, IObservable<PropertyChangedEventArgs>>(x => x.Nickname.PropertyChangedAsObservable()),
                new Action<User>(x => { }), 0)
                .SetName("Nickname Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                new Func<UserViewModel, IObservable<PropertyChangedEventArgs>>(x => x.Nickname.PropertyChangedAsObservable()),
                new Action<User>(x => x.Nickname = Faker.Name.FullName()), 1)
                .SetName("Nickname Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                new Func<UserViewModel, IObservable<PropertyChangedEventArgs>>(x => x.Biography.PropertyChangedAsObservable()),
                new Action<User>(x => { }), 0)
                .SetName("Description Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                new Func<UserViewModel, IObservable<PropertyChangedEventArgs>>(x => x.Biography.PropertyChangedAsObservable()),
                new Action<User>(x => x.Biography = Faker.Lorem.Paragraph()), 1)
                .SetName("Description Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                new Func<UserViewModel, IObservable<PropertyChangedEventArgs>>(x => x.IconUrl.PropertyChangedAsObservable()),
                new Action<User>(x => { }), 0)
                .SetName("IconUrl Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                new Func<UserViewModel, IObservable<PropertyChangedEventArgs>>(x => x.IconUrl.PropertyChangedAsObservable()),
                new Action<User>(x => x.IconUrl = Faker.Url.Image()), 1)
                .SetName("IconUrl Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                new Func<UserViewModel, IObservable<PropertyChangedEventArgs>>(x => x.IconThumbnailUrl.PropertyChangedAsObservable()),
                new Action<User>(x => { }), 0)
                .SetName("IconThumbnailUrl Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                new Func<UserViewModel, IObservable<PropertyChangedEventArgs>>(x => x.IconThumbnailUrl.PropertyChangedAsObservable()),
                new Action<User>(x => x.IconThumbnailUrl = Faker.Url.Image()), 1)
                .SetName("IconThumbnailUrl Property When chnaged Should raise PropertyChanged");
        }

        [Test]
        [TestCaseSource("PropertyChangedTestCase")]
        public void PropertyChanged(
            Func<UserViewModel, IObservable<PropertyChangedEventArgs>> observableGetter,
            Action<User> setter,
            int expected)
        {
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<PropertyChangedEventArgs>();
            var subject = new UserViewModel(model: ModelFixture.User());

            observableGetter(subject)
                .Where(x => x.PropertyName == "Value")
                .Subscribe(observer);

            setter(subject.Model);

            Assert.That(observer.Messages.Count, Is.EqualTo(expected));
        }

        #region RefreshCommand

        [Test]
        public void RefreshCommand_ShouldInvokeFetchUser()
        {
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var fetchUserUseCase = new Mock<IFetchUserUseCase>();
            var model = ModelFixture.User();

            fetchUserUseCase.Setup(x => x.Invoke(It.IsAny<User>())).ReturnsObservable(Unit.Default);

            new UserViewModel(model: model) { SchedulerProvider = schedulerProvider, FetchUserUseCase = fetchUserUseCase.Object }.RefreshCommand.Execute();

            scheduler.AdvanceBy(1); // Invoke UseCase
            fetchUserUseCase.Verify(x => x.Invoke(model), Times.Once);
        }

        [Test]
        public void RefreshCommand_WhenError_ShouldCompleteCommand()
        {
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var fetchUserUseCase = new Mock<IFetchUserUseCase>();
            var model = ModelFixture.User();
            fetchUserUseCase
                .Setup(x => x.Invoke(It.IsAny<User>()))
                .Returns(Observable.Throw<Unit>(new Exception()));

            var subject = new UserViewModel(model: model) { SchedulerProvider = schedulerProvider, FetchUserUseCase = fetchUserUseCase.Object };

            subject.RefreshCommand.Execute();

            scheduler.AdvanceBy(1); // Invoke UseCase

            Assert.That(subject.RefreshCommand.CanExecute(), Is.True);
        }

        #endregion
    }
}
