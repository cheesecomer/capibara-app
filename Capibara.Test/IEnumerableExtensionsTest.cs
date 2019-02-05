#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace Capibara
{
    [TestFixture]
    public class IEnumerableExtensionsTest
    {
        static readonly object[] ForEachTestCases =
            {
                new object[] { Enumerable.Range(10, 10), new[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 } },
                new object[] { null, new int [] { } }
            };

        static readonly object[] ForEachHasIndexTestCases =
            {
                new object[] { null, new Pair<int, int>[] { } },
                new object[]
                {
                    Enumerable.Range(10, 10),
                    Enumerable.Range(0, 10).Select(x => new Pair<int, int>(x, x + 10)).ToArray()
                }
            };

        [TestCaseSource("ForEachTestCases")]
        public void ForEach(IEnumerable<int> enumerable, int[] expected)
        {
            var values = new List<int>();
            enumerable.ForEach(x => values.Add(x));

            Assert.That(values.ToArray(), Is.EqualTo(expected));
        }

        [TestCaseSource("ForEachHasIndexTestCases")]
        public void ForEachHasIndex(IEnumerable<int> enumerable, Pair<int, int>[] expected)
        {
            var values = new List<Pair<int, int>>();
            enumerable.ForEach((i, x) => { values.Add(new Pair<int, int>(i, x)); });

            Assert.That(values.ToArray(), Is.EqualTo(expected));
        }
    }
}
