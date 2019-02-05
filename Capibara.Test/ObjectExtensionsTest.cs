#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System.Collections;
using NUnit.Framework;
using Unity;

namespace Capibara
{
    [TestFixture]
    public class ObjectExtensionsTest
    {
        static IEnumerable IsNullTestCases()
        {
            yield return new TestCaseData(null, true)
                .SetName("IsNull When Null Should Return True");

            yield return new TestCaseData(new object(), false)
                .SetName("IsNull When Present Should Return False");
        }

        static IEnumerable IsPresentTestCases()
        {
            yield return new TestCaseData(null, false)
                .SetName("IsPresent When Null Should Return False");

            yield return new TestCaseData(new object(), true)
                .SetName("IsPresent When Present Should Return True");
        }

        static IEnumerable ToIntTestCases()
        {
            yield return new TestCaseData(null, 0)
                .SetName("ToInt When Null Should Return 0");

            yield return new TestCaseData(new object(), 0)
                .SetName("ToInt When  Should Return 0");

            yield return new TestCaseData("1", 1)
                .SetName("ToInt When Parcelable Should Return 1");

            yield return new TestCaseData("xxxxx", 0)
                .SetName("ToInt When Not Parcelable Should Return 0");
        }

        [TestCase]
        public void BuildUp()
        {
            Assert.DoesNotThrow(() => new object().BuildUp(new UnityContainer()));
        }

        [TestCaseSource("IsNullTestCases")]
        public void IsNull(object value, bool expect)
        {
            Assert.That(value.IsNull(), Is.EqualTo(expect));
        }

        [TestCaseSource("IsPresentTestCases")]
        public void IsPresent(object value, bool expect)
        {
            Assert.That(value.IsPresent(), Is.EqualTo(expect));
        }

        [TestCaseSource("ToIntTestCases")]
        public void ToInt(object value, int expect)
        {
            Assert.That(value.ToInt(), Is.EqualTo(expect));
        }
    }
}
