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
        private bool IsOpenUriCalled;

        private bool IsCancelCalled;

        private bool NeedOpenUriCalled;

        private bool NeedCancel;

        private string Url;

        private string[] Urls;

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
            var deviceService = new Mock<IDeviceService>();
            deviceService.Setup(x => x.OpenUri(It.Is<Uri>(v => v.ToString() == this.Url))).Callback(() => this.IsOpenUriCalled = true);

            var parameters = new Mock<IOverrideUrlCommandParameters>();
            parameters.SetupSet(x => x.Cancel = true).Callback(() => this.IsCancelCalled = true);
            parameters.Setup(x => x.Url).Returns(this.Url);

            (new SubjectClass() as IOverrideUrlService).OverrideUrl(deviceService.Object, this.Urls)?.Invoke(parameters.Object);
        }

        [TestCase]
        public void ItShouldIsOpenUriCalledExpect()
        {
            Assert.That(this.IsOpenUriCalled, Is.EqualTo(this.NeedOpenUriCalled));
        }

        [TestCase]
        public void ItShouldCancelExpect()
        {
            Assert.That(this.IsCancelCalled, Is.EqualTo(this.NeedCancel));
        }
    }
}
