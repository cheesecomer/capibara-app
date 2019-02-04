#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using NUnit.Framework;

namespace Capibara.Test
{
    [TestFixture]
    public class PairTest
    {
        readonly static object[][] EqualsTestCases =
        {
            new object[] { new Pair<string, string>("Foo", "Bar"), true },
            new object[] { new Pair<string, string>("Foo", "Bar!"), false },
            new object[] { new Pair<string, string>("Foo!", "Bar"), false },
            new object[] { new Pair<string, string>("Foo!", "Bar!"), false },
            new object[] { new Pair<string, string>(null, null), false },
            new object[] { new Pair<string, int>("Foo", 100), false },
            new object[] { new Pair<int, string>(100, "Bar"), false },
            new object[] { new Pair<string, int>(null, 100), false },
            new object[] { new Pair<int, string>(100, null), false },
            new object[] { new Pair<int, int>(100, 100), false }
        };

        readonly static object[][] EqualOperatorTestCases =
        {
            new object[] { new Pair<string, string>("Foo", "Bar"), true },
            new object[] { new Pair<string, string>("Foo", "Bar!"), false },
            new object[] { new Pair<string, string>("Foo!", "Bar"), false },
            new object[] { new Pair<string, string>("Foo!", "Bar!"), false },
            new object[] { new Pair<string, string>(null, null), false }
        };

        [TestCase]
        public void FirstProperty()
        {
            var expected = Faker.Lorem.Sentence();
            Assert.That(new Pair<string, string>(expected, string.Empty).First, Is.EqualTo(expected));
        }

        [TestCase]
        public void SecondProperty()
        {
            var expected = Faker.Lorem.Sentence();
            Assert.That(new Pair<string, string>(string.Empty, expected).Second, Is.EqualTo(expected));
        }

        [TestCaseSource("EqualsTestCases")]
        public void Equals(object value, bool expected)
        {
            Assert.That(new Pair<string, string>("Foo", "Bar").Equals(value), Is.EqualTo(expected));
        }

        [TestCaseSource("EqualOperatorTestCases")]
        public void EqualOperator(Pair<string, string> value, bool expected)
        {
            Assert.That(new Pair<string, string>("Foo", "Bar") == value, Is.EqualTo(expected));
        }

        [TestCaseSource("EqualOperatorTestCases")]
        public void NotEqualOperator(Pair<string, string> value, bool expected)
        {
            Assert.That(new Pair<string, string>("Foo", "Bar") != value, Is.EqualTo(!expected));
        }

        [TestCaseSource("EqualsTestCases")]
        public void GetHashCode(object value, bool expected)
        {
            Assert.That(new Pair<string, string>("Foo", "Bar").GetHashCode() == value.GetHashCode(), Is.EqualTo(expected));
        }
    }
}
