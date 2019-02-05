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
    }
}
