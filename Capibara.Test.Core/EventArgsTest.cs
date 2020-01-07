#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using NUnit.Framework;

namespace Capibara
{
    [TestFixture]
    public class EventArgsTest
    {
        [TestCase("", "", TestName = "EventArgs(\"{0}\").Value and {1} should equal")]
        [TestCase("Foo", "Foo", TestName = "EventArgs(\"{0}\").Value and {1} should equal")]
        public void ValueProperty(string value, string expect)
        {
            Assert.That(new EventArgs<string>(value).Value, Is.EqualTo(expect));
        }

        [TestCase("", "", TestName = "EventArgs(\"{0}\") must be convertible to \"{1}\"")]
        [TestCase("Foo", "Foo", TestName = "EventArgs(\"{0}\") must be convertible to \"{1}\"")]
        public void CastSourceProperty(string value, string expect)
        {
            string actual = new EventArgs<string>(value);
            Assert.That(actual, Is.EqualTo(expect));
        }

        [TestCase("", "", TestName = "\"{0}\" must be convertible to EventArgs(\"{1}\")")]
        [TestCase("Foo", "Foo", TestName = "\"{0}\" must be convertible to EventArgs(\"{1}\")")]
        public void CastEventArgsProperty(string value, string expect)
        {
            EventArgs<string> actual = value;
            Assert.That(actual.Value, Is.EqualTo(expect));
        }
    }
}
