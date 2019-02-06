#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using NUnit.Framework;

namespace Capibara.Attributes
{
    public class DisplayNameAttributeTest
    {
        [Test]
        public void DisplayName()
        {
            var expected = Faker.Lorem.Sentence();
            Assert.That(new DisplayNameAttribute(expected).DisplayName, Is.EqualTo(expected));
        }
    }
}
