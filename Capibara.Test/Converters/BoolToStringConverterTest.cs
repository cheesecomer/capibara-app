using NUnit.Framework;

using Capibara.Services;

using Moq;

using Xamarin.Forms;

using Prism.Services;

using SubjectClass = Capibara.Converters.BoolToStringConverter;

namespace Capibara.Test.Converters.BoolToStringConverter
{
    [TestFixture]
    public class Convert
    {
        [TestCase(true, "TRUE!")]
        [TestCase(false, "FALSE!")]
        public void ItShouldResultIsExpect(bool value, string expect)
        {
            Assert.That(new SubjectClass{ TruthyText = "TRUE!", FalsyText = "FALSE!" }.Convert(value, null, null, null), Is.EqualTo(expect));
        }
    }

    public class ConvertBack
    {
        [TestCase("TRUE!", true)]
        [TestCase("FALSE!", false)]
        public void ItShouldThrowNotSupportedException(string value, bool expect)
        {
            Assert.That(new SubjectClass { TruthyText = "TRUE!", FalsyText = "FALSE!" }.ConvertBack(value, null, null, null), Is.EqualTo(expect));
        }
    }
}
