﻿#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using NUnit.Framework;
using Unity;

namespace Capibara.Test
{
    [TestFixture]
    public class ObjectExtensionsTest
    {
        static readonly object[] IsNullTestCases =
            {
                new object[] { null, true },
                new object[] { new object(), false }
            };

        static readonly object[] IsPresentTestCases =
            {
                new object[] { null, false },
                new object[] { new object(), true }
            };

        static readonly object[] ToIntTestCases =
            {
                new object[] { null, 0 },
                new object[] { new object(), 0 },
                new object[] { "1", 1 },
                new object[] { "1 aaa", 0 }
            };

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
