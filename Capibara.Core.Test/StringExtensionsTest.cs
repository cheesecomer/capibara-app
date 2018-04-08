using System;

using NUnit.Framework;

namespace Capibara.Test
{
    [TestFixture]
    public class StringExtensionsTest
    {
        [TestCase("\r", "")]
        [TestCase("\n", "")]
        [TestCase("\r\n", "")]
        [TestCase(" ", "")]
        [TestCase(" \r\n ", "")]
        public void ToSlimTest(string value, string expect)
        {
            Assert.That(value.ToSlim(), Is.EqualTo(expect));
        }

        [TestCase("", true)]
        [TestCase(null, true)]
        [TestCase("Foo", false)]
        public void IsNullOrEmptyTest(string value, bool expect)
        {
            Assert.That(value.IsNullOrEmpty(), Is.EqualTo(expect));
        }

        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase("Foo", true)]
        public void IsPresentTest(string value, bool expect)
        {
            Assert.That(value.IsPresent(), Is.EqualTo(expect));
        }

        [TestCase("", null)]
        [TestCase(null, null)]
        [TestCase("Foo", "Foo")]
        public void PresenceTest(string value, string expect)
        {
            Assert.That(value.Presence(), Is.EqualTo(expect));
        }
    }
}
