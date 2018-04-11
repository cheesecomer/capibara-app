using System;
using NUnit.Framework;

using Subject = Capibara.FailEventArgs;

namespace Capibara.Test.FailEventArgs
{
    public class ErrorPropertyTest
    {
        [TestCase("", "")]
        [TestCase("Foo", "Foo")]
        public void ItShouldExpect(string message, string expect)
        {
            var exseption = new Exception(message);
            Assert.That(new Subject(exseption).Error.Message, Is.EqualTo(expect));
        }
    }

    public class CastFailEventArgsropertyTest
    {
        [TestCase("", "")]
        [TestCase("Foo", "Foo")]
        public void ItShouldExpect(string message, string expect)
        {
            Subject actual = new Exception(message);
            Assert.That(actual.Error.Message, Is.EqualTo(expect));
        }
    }
}