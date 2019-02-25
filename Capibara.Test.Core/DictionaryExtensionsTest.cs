#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System.Collections.Generic;

using NUnit.Framework;

namespace Capibara
{
    [TestFixture]
    public class DictionaryExtensionsTest
    {
        [TestCase("Foo", null, "Bar", TestName = "{m}({0}, {1}) Is When exists key Should return value")]
        [TestCase("foo", null, null, TestName = "{m}({0}, {1}) Is When not exists key Should return null")]
        [TestCase("Foo!", "Bar!", "Bar!", TestName = "{m}({0}, {1}) Is When not exists key and set default value Should return defauld value")]
        public void ValueOrDefault(string value, string defaultValue, string expected)
        {
            var dictionary = new Dictionary<string, string> { { "Foo", "Bar" } };

            Assert.That(dictionary.ValueOrDefault(value, defaultValue), Is.EqualTo(expected));
        }
    }
}
