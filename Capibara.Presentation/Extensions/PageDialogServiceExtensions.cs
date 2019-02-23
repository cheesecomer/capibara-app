using System;
using System.Threading.Tasks;
using Prism.Services;

namespace Capibara.Presentation
{
    public static class PageDialogServiceExtensions
    {
        public static Task<bool> DisplayErrorAlertAsync(this IPageDialogService pageDialogService, Exception exception)
        {
            return pageDialogService.DisplayAlertAsync(
                        "申し訳ございません！",
                        "通信エラーです。リトライしますか？。",
                        "リトライ",
                        "閉じる");
        }
    }
}
