using System;
using NUnit.Framework;

using Capibara.Converters;
namespace Capibara.Test.Converters.DoubleMultiplierConverterTest
{
    public class Convert
    {
        [TestCase(1, "", 1d)]
        [TestCase(1, "2", 2d)]
        [TestCase(1f, "", 1d)]
        [TestCase(1f, "2", 2d)]
        [TestCase(1d, "", 1d)]
        [TestCase(1d, "2", 2d)]
        public void ItShouldResultIsExpect(object value, object parameter, double expect)
        {
            Assert.That(new DoubleMultiplierConverter().Convert(value, null, parameter, null), Is.EqualTo(expect));
        }
    }

    public class ConvertBack
    {
        public void ItShouldThrowNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => new DoubleMultiplierConverter().ConvertBack(null, null, null, null));
        }
    }
}
