#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Collections;
using NUnit.Framework;

namespace Capibara.Domain.Models
{
    public class MessageTest
    {
        static private IEnumerable RestoreTestCase()
        {
            yield return new TestCaseData(
                new Action<Message, Message>((actual, expected) => Assert.That(actual.Restore(expected).Id, Is.EqualTo(expected.Id))))
                .SetName("Restore Should Id is to be restored");

            yield return new TestCaseData(
                new Action<Message, Message>((actual, expected) => Assert.That(actual.Restore(expected).Content, Is.EqualTo(expected.Content))))
                .SetName("Restore Should Content is to be restored");

            yield return new TestCaseData(
                new Action<Message, Message>((actual, expected) => Assert.That(actual.Restore(expected).At, Is.EqualTo(expected.At))))
                .SetName("Restore Should At is to be restored");

            yield return new TestCaseData(
                new Action<Message, Message>((actual, expected) => Assert.That(actual.Restore(expected).Sender, Is.EqualTo(expected.Sender))))
                .SetName("Restore Should Sender is to be restored");

            yield return new TestCaseData(
                new Action<Message, Message>((actual, expected) => Assert.That(actual.Restore(expected).ImageUrl, Is.EqualTo(expected.ImageUrl))))
                .SetName("Restore Should ImageUrl is to be restored");

            yield return new TestCaseData(
                new Action<Message, Message>((actual, expected) => Assert.That(actual.Restore(expected).ImageThumbnailUrl, Is.EqualTo(expected.ImageThumbnailUrl))))
                .SetName("Restore Should ImageThumbnailUrl is to be restored");
        }

        [Test]
        [TestCaseSource("RestoreTestCase")]
        public void Restore(Action<Message, Message> assert)
        {
            assert.Invoke(new Message(), ModelFixture.Message());
        }
    }
}
