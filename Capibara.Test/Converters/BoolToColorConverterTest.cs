using NUnit.Framework;

using Capibara.Services;

using Moq;

using Xamarin.Forms;

using Prism.Services;

using SubjectClass = Capibara.Converters.BoolToColorConverter;

namespace Capibara.Test.Converters.BoolToColorConverter
{
    [TestFixture]
    public class Convert
    {
        static object[] TestCaseSource =
        {
            new object[] { true, Color.Red },
            new object[] { false, Color.Blue }
        };

        [TestCaseSource("TestCaseSource")]
        public void ItShouldResultIsExpect(bool value, Color expect)
        {
            Assert.That(new SubjectClass { TruthyColor = Color.Red, FalsyColor = Color.Blue }.Convert(value, null, null, null), Is.EqualTo(expect));
        }
    }

    public class ConvertBack
    {
        static object[] TestCaseSource =
        {
            new object[] { Color.Red, true },
            new object[] { Color.Blue, false }
        };

        [TestCaseSource("TestCaseSource")]
        public void ItShouldThrowNotSupportedException(Color value, bool expect)
        {
            Assert.That(new SubjectClass { TruthyColor = Color.Red, FalsyColor = Color.Blue }.ConvertBack(value, null, null, null), Is.EqualTo(expect));
        }
    }
}
