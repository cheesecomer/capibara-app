#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace Capibara.Test
{
    public class Pair<TFirst, TSecond>
    {
        public TFirst First { get; }
        public TSecond Second { get; }

        public Pair(TFirst first, TSecond second)
        {
            this.First = first;
            this.Second = second;
        }

        public override bool Equals(object obj)
        {
            if (obj == this) return true;

            if (obj is Pair<TFirst, TSecond> target)
            {
                if (!target.First.Equals(this.First)) return false;
                if (!target.Second.Equals(this.Second)) return false;

                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"({this.First} to {this.Second})";
        }
    }

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
