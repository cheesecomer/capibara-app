using System;
using NUnit.Framework;

using Capibara.ViewModels;

using SubjectClass = Capibara.Converters.WebNavigatingEventArgsConverter;

namespace Capibara.Test.Converters.WebNavigatingEventArgsConverter
{
    [TestFixture]
    public class Convert
    {
        public void ItShouldResultIsExpect()
        {
            Assert.That(new SubjectClass().Convert(new object(), null, null, null), Is.TypeOf<IOverrideUrlCommandParameters>());
        }
    }

    public class ConvertBack
    {
        [TestCase]
        public void ItShouldThrowNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => new SubjectClass().ConvertBack(null, null, null, null));
        }
    }
}
