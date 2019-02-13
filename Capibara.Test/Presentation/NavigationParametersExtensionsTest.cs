#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using NUnit.Framework;
using Prism.Navigation;

namespace Capibara.Presentation
{
    [TestFixture]
    public class NavigationParametersExtensionsTest
    {
        [Test]
        [TestCase("Foo", "Bar", TestName = "TryGetValue WhenExistsKey ShouldReturnValue")]
        [TestCase("Foo!", null, TestName = "TryGetValue WhenNotExistsKey ShouldReturnDefaultValue")]
        public void TryGetValue(string key, string expected)
        {
            Assert.That(new NavigationParameters { { "Foo", "Bar" } }.TryGetValue<string>(key), Is.EqualTo(expected));
        }
    }
}
