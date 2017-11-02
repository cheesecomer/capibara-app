using System;
using NUnit.Framework;

using Capibara.Converters;
namespace Capibara.Test.Converters.DoubleMultiplierConverterTest
{
    [TestFixture]
    public class Convert
    {
        [TestCase(1, "", 1d)]
        [TestCase(1, "2", 2d)]
        [TestCase(1f, "", 1d)]
        [TestCase(1f, "2", 2d)]
        [TestCase(1d, "", 1d)]
        [TestCase(1d, "2", 2d)]
        [TestCase("", "2", -1d)]
        public void ItShouldResultIsExpect(object value, object parameter, double expect)
        {
            Assert.That(new DoubleMultiplierConverter().Convert(value, null, parameter, null), Is.EqualTo(expect));
        }
    }

    public class ConvertBack
    {
        [TestCase]
        public void ItShouldThrowNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => new DoubleMultiplierConverter().ConvertBack(null, null, null, null));
        }
    }
}
