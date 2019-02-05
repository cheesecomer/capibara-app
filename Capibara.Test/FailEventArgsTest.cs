#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using NUnit.Framework;

namespace Capibara
{
    public class FailEventArgsTest
    {
        [TestCase("", "")]
        [TestCase("Foo", "Foo")]
        public void ErrorProperty(string message, string expect)
        {
            var exseption = new Exception(message);
            Assert.That(new FailEventArgs(exseption).Error.Message, Is.EqualTo(expect));
        }

        [TestCase("", "")]
        [TestCase("Foo", "Foo")]
        public void CastFailEventArgsroperty(string message, string expect)
        {
            FailEventArgs actual = new Exception(message);
            Assert.That(actual.Error.Message, Is.EqualTo(expect));
        }
    }
}