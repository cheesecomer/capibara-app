#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using NUnit.Framework;

namespace Capibara
{
    [TestFixture]
    public class StringExtensionsTest
    {
        [TestCase("　", "　")]
        [TestCase("\r", "")]
        [TestCase("\n", "")]
        [TestCase("\r\n", "")]
        [TestCase(" ", "")]
        [TestCase(" \r\n ", "")]
        public void ToSlim(string value, string expect)
        {
            Assert.That(value.ToSlim(), Is.EqualTo(expect));
        }

        [TestCase("", true, TestName = "\"{0}\".{m}() should {1}")]
        [TestCase(null, true, TestName = "\"{0}\".{m}() should {1}")]
        [TestCase("Foo", false, TestName = "\"{0}\".{m}() should {1}")]
        public void IsNullOrEmpty(string value, bool expect)
        {
            Assert.That(value.IsNullOrEmpty(), Is.EqualTo(expect));
        }

        [TestCase("", false, TestName = "\"{0}\".{m}() should {1}")]
        [TestCase(null, false, TestName = "\"{0}\".{m}() should {1}")]
        [TestCase("Foo", true, TestName = "\"{0}\".{m}() should {1}")]
        public void IsPresent(string value, bool expect)
        {
            Assert.That(value.IsPresent(), Is.EqualTo(expect));
        }

        [TestCase("", null, TestName = "\"{0}\".{m}() should {1}")]
        [TestCase(null, null, TestName = "\"{0}\".{m}() should {1}")]
        [TestCase("Foo", "Foo", TestName = "\"{0}\".{m}() should {1}")]
        public void Presence(string value, string expect)
        {
            Assert.That(value.Presence(), Is.EqualTo(expect));
        }
    }
}
