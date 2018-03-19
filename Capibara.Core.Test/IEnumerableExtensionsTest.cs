using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace Capibara.Test.IEnumerableExtensionsTest
{
    [TestFixture]
    public class ForEachTest
    {
        [TestCase]
        public void WhenNotNull()
        {
            var values = new List<int>();
            Enumerable.Range(10, 10).ForEach(x => values.Add(x));

            Assert.That(values.ToArray(), Is.EqualTo(new[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 }));
        }

        [TestCase]
        public void WhenNull()
        {
            var values = new List<int>();
            ((IEnumerable<int>)null).ForEach(x => values.Add(x));

            Assert.That(values.Count, Is.EqualTo(0));
        }
    }

    [TestFixture]
    public class ForEachHasIndexTest
    {
        [TestCase]
        public void WhenNotNull()
        {
            var values = new List<int>();
            var indexs = new List<int>();
            Enumerable.Range(10, 10).ForEach((i, x) => { indexs.Add(i); values.Add(x); });

            Assert.That(values.ToArray(), Is.EqualTo(new[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 }));
            Assert.That(indexs.ToArray(), Is.EqualTo(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }));
        }

        [TestCase]
        public void WhenNull()
        {
            var values = new List<int>();
            ((IEnumerable<int>)null).ForEach((i, x) => values.Add(x));

            Assert.That(values.Count, Is.EqualTo(0));
        }
    }
}
