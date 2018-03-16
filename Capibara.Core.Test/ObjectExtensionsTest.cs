using System;

using NUnit.Framework;

using Unity;
namespace Capibara.Test
{
    [TestFixture]
    public class ObjectExtensionsTest
    {
        static object[] IsNullTestCases =
            {
                new object[] { null, true },
                new object[] { new object(), false }
            };

        static object[] IsPresentTestCases =
            {
                new object[] { null, false },
                new object[] { new object(), true }
        };

        static object[] ToIntTestCases =
            {
                new object[] { null, 0 },
                new object[] { new object(), 0 },
                new object[] { "1", 1 },
                new object[] { "1 aaa", 0 }
        };

        [TestCase]
        public void BuildUpTest()
        {
            Assert.DoesNotThrow(() => new object().BuildUp(new UnityContainer()));
        }

        [TestCaseSource("IsNullTestCases")]
        public void IsNullTest(object value, bool expect)
        {
            Assert.That(value.IsNull(), Is.EqualTo(expect));
        }

        [TestCaseSource("IsPresentTestCases")]
        public void IsPresentTest(object value, bool expect)
        {
            Assert.That(value.IsPresent(), Is.EqualTo(expect));
        }

        [TestCaseSource("ToIntTestCases")]
        public void ToIntTest(object value, int expect)
        {
            Assert.That(value.ToInt(), Is.EqualTo(expect));
        }
    }
}
