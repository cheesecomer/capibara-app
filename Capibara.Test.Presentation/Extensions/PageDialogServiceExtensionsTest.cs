using System;
using Prism.Services;
using NUnit.Framework;
using Moq;

namespace Capibara.Presentation
{
    [TestFixture]
    public class PageDialogServiceExtensionsTest
    {
        [Test]
        public void DisplayErrorAlertAsyncTest()
        {
            var pageDialogService = new Mock<IPageDialogService>();

            pageDialogService.Object.DisplayErrorAlertAsync(new Exception());

            pageDialogService.Verify(
                x => x.DisplayAlertAsync(
                    "申し訳ございません！",
                    "通信エラーです。リトライしますか？。",
                    "リトライ",
                    "閉じる"),
                Times.Once);
        }
    }
}
