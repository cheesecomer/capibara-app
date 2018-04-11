using System;
using System.Linq;

using Capibara.Services;
using Capibara.ViewModels;

using Moq;
using NUnit.Framework;

using Prism.Services;

using Xamarin.Forms;

using SubjectClass = Capibara.Services.OverrideUrlService;

namespace Capibara.Test.Service.OverrideUrlService
{
    [TestFixture(new[] { "http://foobar.com" }, "http://foobar.com/", false, false, TestName = "OverrideUrlTest(http://foobar.com)")]
    [TestFixture(new[] { "http://foobar.co.jp" }, "http://foobar.com/", true, true, TestName = "OverrideUrlTest(http://foobar.co.jp)")]
    [TestFixture(new[] { "http://foobar.com", "http://foobar.co.jp" }, "http://foobar.com/", false, false, TestName = "OverrideUrlTest(http://foobar.com, http://foobar.co.jp)")]
    [TestFixture(new[] { "http://foobar.net", "http://foobar.co.jp" }, "http://foobar.com/", true, true, TestName = "OverrideUrlTest(http://foobar.net, http://foobar.co.jp)")]
    public class OverrideUrlTest
    {
        private bool NeedOpenUriCalled;

        private bool NeedCancel;

        private string Url;

        private string[] Urls;

        Mock<IDeviceService> DeviceService;

        Mock<IOverrideUrlCommandParameters> Parameters;

        public OverrideUrlTest(string[] urls, string url, bool needOpenUriCalled, bool needCancel)
        {
            this.Urls = urls;
            this.Url = url;
            this.NeedOpenUriCalled = needOpenUriCalled;
            this.NeedCancel = needCancel;
        }

        [SetUp]
        public void SetUp()
        {
            this.DeviceService = new Mock<IDeviceService>();
            this.DeviceService.Setup(x => x.OpenUri(It.IsAny<Uri>()));

            this.Parameters = new Mock<IOverrideUrlCommandParameters>();
            this.Parameters.Setup(x => x.Url).Returns(this.Url);

            (new SubjectClass() as IOverrideUrlService).OverrideUrl(this.DeviceService.Object, this.Urls)?.Invoke(this.Parameters.Object);
        }

        [TestCase]
        public void ItShouldIsOpenUriCalledExpect()
        {
            this.DeviceService.Verify(
                x => x.OpenUri(It.Is<Uri>(v => v.ToString() == this.Url)),
                this.NeedOpenUriCalled ? Times.Once() : Times.Never());
        }

        [TestCase]
        public void ItShouldCancelExpect()
        {
            this.Parameters.VerifySet(x => x.Cancel = true, this.NeedCancel ? Times.Once() : Times.Never());
        }
    }
}
