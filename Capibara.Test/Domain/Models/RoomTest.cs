#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Collections;
using System.ComponentModel;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using Reactive.Bindings.Extensions;

namespace Capibara.Domain.Models
{
    [TestFixture]
    public class RoomTest
    {
        static private IEnumerable RestoreTestCase()
        {
            yield return new TestCaseData(
                new Action<Room, Room>((actual, expected) => Assert.That(actual.Restore(expected).Id, Is.EqualTo(expected.Id))))
                .SetName("Restore Should Id is to be restored");

            yield return new TestCaseData(
                new Action<Room, Room>((actual, expected) => Assert.That(actual.Restore(expected).Name, Is.EqualTo(expected.Name))))
                .SetName("Restore Should Name is to be restored");

            yield return new TestCaseData(
                new Action<Room, Room>((actual, expected) => Assert.That(actual.Restore(expected).Capacity, Is.EqualTo(expected.Capacity))))
                .SetName("Restore Should Capacity is to be restored");

            yield return new TestCaseData(
                new Action<Room, Room>((actual, expected) => Assert.That(actual.Restore(expected).NumberOfParticipants, Is.EqualTo(expected.NumberOfParticipants))))
                .SetName("Restore Should NumberOfParticipants is to be restored");

            yield return new TestCaseData(
                new Action<Room, Room>((actual, expected) => Assert.That(actual.Restore(expected).IsConnected, Is.EqualTo(expected.IsConnected))))
                .SetName("Restore Should IsConnected is to be restored");

            yield return new TestCaseData(
                new Action<Room, Room>((actual, expected) => Assert.That(actual.Restore(expected).Messages, Is.EqualTo(expected.Messages))))
                .SetName("Restore Should Messages is to be restored");

            yield return new TestCaseData(
                new Action<Room, Room>((actual, expected) => Assert.That(actual.Restore(expected).Participants, Is.EqualTo(expected.Participants))))
                .SetName("Restore Should Participants is to be restored");
        }

        [Test]
        [TestCaseSource("RestoreTestCase")]
        public void Restore(Action<Room, Room> assert)
        {
            assert.Invoke(new Room(), ModelFixture.Room());
        }

        [Test]
        public new void ToString()
        {
            var subject = ModelFixture.Room();
            var expect = $"{{ Id: {subject.Id}, Name: {subject.Name}, Capacity: {subject.Capacity}, NumberOfParticipants: {subject.NumberOfParticipants} }}";
            Assert.That(subject.ToString(), Is.EqualTo(expect));
        }

        [Test]
        public new void GetHashCode()
        {
            var subject = ModelFixture.Room();
            Assert.That(subject.GetHashCode(), Is.EqualTo(subject.Id.GetHashCode()));
        }

        static private IEnumerable EqualsTestCase()
        {
            yield return new TestCaseData(
                new Func<Room, object>(_ => null),
                false)
                .SetName("Equals When Null Should Return False");

            yield return new TestCaseData(
                new Func<Room, object>(_ => new object()),
                false)
                .SetName("Equals When Not Room Should Return False");

            yield return new TestCaseData(
                new Func<Room, object>(v => 
                    new Room 
                    {
                        Id = -1,
                        Capacity = v.Capacity,
                        Name = v.Name,
                        NumberOfParticipants = v.NumberOfParticipants
                    }),
                false)
                .SetName("Equals When Id are not equal Should Return False");

            yield return new TestCaseData(
                new Func<Room, object>(v =>
                    new Room
                    {
                        Id = v.Id,
                        Capacity = v.Capacity,
                        Name = string.Empty,
                        NumberOfParticipants = v.NumberOfParticipants
                    }),
                false)
                .SetName("Equals When Name are not equal Should Return False");

            yield return new TestCaseData(
                new Func<Room, object>(v =>
                    new Room
                    {
                        Id = v.Id,
                        Capacity = -1,
                        Name = v.Name,
                        NumberOfParticipants = v.NumberOfParticipants
                    }),
                false)
                .SetName("Equals When Capacity are not equal Should Return False");

            yield return new TestCaseData(
                new Func<Room, object>(v =>
                    new Room
                    {
                        Id = v.Id,
                        Capacity = v.Capacity,
                        Name = v.Name,
                        NumberOfParticipants = -1
                    }),
                false)
                .SetName("Equals When NumberOfParticipants are not equal Should Return False");
        }

        [Test]
        [TestCaseSource("EqualsTestCase")]
        public void Equals(Func<Room, object> creater, bool expected)
        {
            var subject = ModelFixture.Room();
            var other = creater(subject);

            Assert.That(subject.Equals(other), Is.EqualTo(expected));
        }


        static private IEnumerable PropertyChangedTestCase()
        {
            yield return new TestCaseData(
                "Id", new Action<Room>(x => { }), 0)
                .SetName("Id Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "Id", new Action<Room>(x => x.Id = x.Id + 1), 1)
                .SetName("Id Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                "Name", new Action<Room>(x => { }), 0)
                .SetName("Name Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "Name", new Action<Room>(x => x.Name = $"{x.Name} !"), 1)
                .SetName("Name Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                "Capacity", new Action<Room>(x => { }), 0)
                .SetName("Capacity Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "Capacity", new Action<Room>(x => x.Capacity = x.Capacity + 1), 1)
                .SetName("Capacity Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                "NumberOfParticipants", new Action<Room>(x => { }), 0)
                .SetName("NumberOfParticipants Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "NumberOfParticipants", new Action<Room>(x => x.NumberOfParticipants = x.NumberOfParticipants + 1), 1)
                .SetName("NumberOfParticipants Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                "IsConnected", new Action<Room>(x => { }), 0)
                .SetName("IsConnected Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "IsConnected", new Action<Room>(x => x.IsConnected = !x.IsConnected), 1)
                .SetName("IsConnected Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                "Messages", new Action<Room>(x => { }), 0)
                .SetName("Messages Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "Messages", new Action<Room>(x => x.Messages.Add(ModelFixture.Message())), 1)
                .SetName("Messages Property When add Should raise PropertyChanged");

            yield return new TestCaseData(
                "Messages", new Action<Room>(x => x.Messages.RemoveAt(0)), 1)
                .SetName("Messages Property When remove Should raise PropertyChanged");

            yield return new TestCaseData(
                "Participants", new Action<Room>(x => { }), 0)
                .SetName("Participants Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                "Participants", new Action<Room>(x => x.Participants.Add(ModelFixture.User())), 1)
                .SetName("Participants Property When add Should raise PropertyChanged");

            yield return new TestCaseData(
                "Participants", new Action<Room>(x => x.Participants.RemoveAt(0)), 1)
                .SetName("Participants Property When remove Should raise PropertyChanged");
        }

        [Test]
        [TestCaseSource("PropertyChangedTestCase")]
        public void PropertyChanged(string propertyName, Action<Room> setter, int expected)
        {
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<PropertyChangedEventArgs>();
            var subject = ModelFixture.Room();
            subject.PropertyChangedAsObservable()
                .Where(x => x.PropertyName == propertyName)
                .Subscribe(observer);

            setter.Invoke(subject);

            Assert.That(observer.Messages.Count, Is.EqualTo(expected));
        }
    }
}
