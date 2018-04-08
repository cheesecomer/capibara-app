using NUnit.Framework;

namespace Capibara.Test.EventArgs
{
    public class ValuePropertyTest
    {
        [TestCase("", "")]
        [TestCase("Foo", "Foo")]
        public void ItShouldExpect(string value, string expect)
        {
            Assert.That(new EventArgs<string>(value).Value, Is.EqualTo(expect));
        }
    }

    public class CastSourcePropertyTest
    {
        [TestCase("", "")]
        [TestCase("Foo", "Foo")]
        public void ItShouldExpect(string value, string expect)
        {
            string actual = new EventArgs<string>(value);
            Assert.That(actual, Is.EqualTo(expect));
        }
    }

    public class CastEventArgsropertyTest
    {
        [TestCase("", "")]
        [TestCase("Foo", "Foo")]
        public void ItShouldExpect(string value, string expect)
        {
            EventArgs<string> actual = value;
            Assert.That(actual.Value, Is.EqualTo(expect));
        }
    }
}
