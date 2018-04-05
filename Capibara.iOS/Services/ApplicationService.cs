using System;
using Capibara.Services;

using Foundation;

namespace Capibara.iOS.Services
{
    public class ApplicationService : IApplicationService
    {
        void IApplicationService.Exit() => System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();

        string IApplicationService.StoreUrl => "https://itunes.apple.com/jp/app/capibara/id1367296814?l=ja&ls=1&mt=8";

        string IApplicationService.Platform => "iOS";

        string IApplicationService.AppVersion => NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"].ToString();
    }
}
