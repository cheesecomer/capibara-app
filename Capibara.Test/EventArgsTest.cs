#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using NUnit.Framework;

namespace Capibara.Test
{
    public class EventArgsTest
    {
        [TestCase("", "")]
        [TestCase("Foo", "Foo")]
        public void ValueProperty(string value, string expect)
        {
            Assert.That(new EventArgs<string>(value).Value, Is.EqualTo(expect));
        }

        [TestCase("", "")]
        [TestCase("Foo", "Foo")]
        public void CastSourceProperty(string value, string expect)
        {
            string actual = new EventArgs<string>(value);
            Assert.That(actual, Is.EqualTo(expect));
        }

        [TestCase("", "")]
        [TestCase("Foo", "Foo")]
        public void CastEventArgsroperty(string value, string expect)
        {
            EventArgs<string> actual = value;
            Assert.That(actual.Value, Is.EqualTo(expect));
        }
    }
}
