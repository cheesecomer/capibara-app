#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Collections;
using NUnit.Framework;

namespace Capibara.Domain.Models
{
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
    }
}
