using System;
using System.Collections.Generic;

using Capibara;

using NUnit.Framework;

namespace Capibara.Test
{
    [TestFixture]
    public class DictionaryExtensionsTest
    {
        [TestCase("Foo", "Bar")]
        [TestCase("foo", null)]
        public void ValueOrDefaultTest(string value, string expect)
        {
            var dictionary = new Dictionary<string, string>() { { "Foo", "Bar" } };

            Assert.That(dictionary.ValueOrDefault(value), Is.EqualTo(expect));
        }
    }
}
