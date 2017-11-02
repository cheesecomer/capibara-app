using System;
using NUnit.Framework;

using Capibara.Converters;

namespace Capibara.Test.Converters.FloatMultiplierConverterTest
{
    public class Convert
    {
        [TestCase(1, "", 1f)]
        [TestCase(1, "2", 2f)]
        [TestCase(1f, "", 1f)]
        [TestCase(1f, "2", 2f)]
        [TestCase(1d, "", 1f)]
        [TestCase(1d, "2", 2f)]
        [TestCase(-1, "", 0f)]
        [TestCase(-1, "2", 0f)]
        [TestCase(-1f, "", 0f)]
        [TestCase(-1f, "2", 0f)]
        [TestCase(-1d, "", 0f)]
        [TestCase(-1d, "2", 0f)]
        public void ItShouldResultIsExpect(object value, object parameter, double expect)
        {
            Assert.That(new FloatMultiplierConverter().Convert(value, null, parameter, null), Is.EqualTo(expect));
        }
    }

    public class ConvertBack
    {
        public void ItShouldThrowNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => new FloatMultiplierConverter().ConvertBack(null, null, null, null));
        }
    }
}
