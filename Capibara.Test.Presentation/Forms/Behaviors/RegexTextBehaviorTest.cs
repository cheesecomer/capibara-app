#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using NUnit.Framework;
using Xamarin.Forms;

namespace Capibara.Presentation.Forms.Behaviors
{
    public class RegexTextBehaviorTest
    {
        [Test]
        public void OnAttachedTo()
        {
            var entry = new Entry();
            var subject = new RegexTextBehavior();

            Assert.DoesNotThrow(() => entry.Behaviors.Add(subject));
        }

        [Test]
        public void OnDetachingFrom()
        {
            var entry = new Entry();
            var subject = new RegexTextBehavior();

            entry.Behaviors.Add(subject);
            Assert.DoesNotThrow(() => entry.Behaviors.Remove(subject));
        }

        [Test]
        [TestCase(null, "1234567890", "1234567890", TestName = "OnTextChange WhenNullToValid ShouldChange")]
        [TestCase(null, "ABCDEFGHIJ", "", TestName = "OnTextChange WhenNullToInvalid ShouldEmpty")]
        [TestCase(null, "", "", TestName = "OnTextChange WhenNullToEmpty ShouldEmpty")]
        [TestCase("", "1234567890", "1234567890", TestName = "OnTextChange WhenEmptyToValid ShouldChange")]
        [TestCase("", "ABCDEFGHIJ", "", TestName = "OnTextChange WhenEmptyToInvalid ShouldEmpty")]
        [TestCase("", null, "", TestName = "OnTextChange WhenEmptyToNull ShouldEmpty")]
        [TestCase("1234567890", "0987654321", "0987654321", TestName = "OnTextChange WhenValidToValid ShouldChange")]
        [TestCase("1234567890", "ABCDEFGHIJ", "1234567890", TestName = "OnTextChange WhenValidToInvalid ShouldRollback")]
        [TestCase("1234567890", "", "", TestName = "OnTextChange WhenValidToEmpty ShouldEmpty")]
        [TestCase("1234567890", null, "", TestName = "OnTextChange WhenValidToNull ShouldEmpty")]
        [TestCase("ABCDEFGHIJ", "1234567890", "1234567890", TestName = "OnTextChange WhenInvalidToValid ShouldChange")]
        [TestCase("ABCDEFGHIJ", "KLMNOPQRST", "", TestName = "OnTextChange WhenInvalidToInvalid ShouldEmpty")]
        [TestCase("ABCDEFGHIJ", "", "", TestName = "OnTextChange WhenInvalidToEmpty ShouldEmpty")]
        [TestCase("ABCDEFGHIJ", null, "", TestName = "OnTextChange WhenInvalidToNull ShouldEmpty")]
        public void OnTextChange_InvalidToInvalid(string before, string after, string expected)
        {
            var entry = new Entry { Text = before };
            entry.Behaviors.Add(new RegexTextBehavior { RegexPattern = "^[0-9]*$" });

            entry.Text = after;
            Assert.That(entry.Text, Is.EqualTo(expected));
        }
    }
}
