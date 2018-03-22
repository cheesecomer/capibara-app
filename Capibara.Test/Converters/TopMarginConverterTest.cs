using System;
using NUnit.Framework;

using Capibara.Services;

using Moq;

using Xamarin.Forms;

using Prism.Services;

using SubjectClass = Capibara.Converters.TopMarginConverter;

namespace Capibara.Test.Converters.TopMarginConverter
{
    [TestFixture]
    public class Convert
    {
        [TestCase(Device.Android, 1024, 0)]
        [TestCase(Device.iOS, 1024, 20)]
        [TestCase(Device.iOS, 2436, 44)]
        public void ItShouldResultIsExpect(string platform, int height, int expect)
        {
            var deviceService = new Mock<IDeviceService>();
            deviceService.SetupGet(x => x.DeviceRuntimePlatform).Returns(platform);

            var screenService = new Mock<IScreenService>();
            screenService.SetupGet(x => x.Size).Returns(new Size(0, height));

            Assert.That(new SubjectClass(deviceService.Object, screenService.Object).Convert(null, null, null, null), Is.EqualTo(expect));
        }
    }

    public class ConvertBack
    {
        [TestCase]
        public void ItShouldThrowNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => new SubjectClass(null, null).ConvertBack(null, null, null, null));
        }
    }
}
