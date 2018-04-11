using System;
using NUnit.Framework;

using Capibara.ViewModels;

using Xamarin.Forms;

using SubjectClass = Capibara.Converters.WebNavigatingEventArgsConverter;

namespace Capibara.Test.Converters.WebNavigatingEventArgsConverter
{
    [TestFixture]
    public class Convert
    {
        [TestCase]
        public void ItShouldResultIsExpect()
        {
            Assert.That(new SubjectClass().Convert(new WebNavigatingEventArgs(WebNavigationEvent.NewPage, null, ""), null, null, null), Is.InstanceOf<IOverrideUrlCommandParameters>());
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
