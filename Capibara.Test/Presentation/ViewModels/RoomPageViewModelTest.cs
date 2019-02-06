#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Capibara.Domain.Models;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using Reactive.Bindings.Extensions;

namespace Capibara.Presentation.ViewModels
{
    public class RoomPageViewModelTest
    {
        static private IEnumerable Property_WhenRisePropertyChanged_ShouldUpdate_TestCase()
        {
            yield return new TestCaseData(
                new Func<Room, object>(x => x.Name = Faker.Lorem.Sentence()),
                new Func<RoomPageViewModel, object>(x => x.Name.Value))
                .SetName("Name Property When changed Should change");

            yield return new TestCaseData(
                new Func<Room, object>(x => x.Capacity = Faker.RandomNumber.Next(10, 50)),
                new Func<RoomPageViewModel, object>(x => x.Capacity.Value))
                .SetName("Capacity Property When changed Should change");

            yield return new TestCaseData(
                new Func<Room, object>(x => x.NumberOfParticipants = Faker.RandomNumber.Next(10, 50)),
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
                new Action<Room>(x => x.Capacity = Faker.RandomNumber.Next(10, 50)), 1)
                .SetName("Capacity Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                new Func<RoomPageViewModel, IObservable<PropertyChangedEventArgs>>(x => x.NumberOfParticipants.PropertyChangedAsObservable()),
                new Action<Room>(x => { }), 0)
                .SetName("NumberOfParticipants Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                new Func<RoomPageViewModel, IObservable<PropertyChangedEventArgs>>(x => x.NumberOfParticipants.PropertyChangedAsObservable()),
                new Action<Room>(x => x.NumberOfParticipants = Faker.RandomNumber.Next(10, 50)), 1)
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
    }
}
