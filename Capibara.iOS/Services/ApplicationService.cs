using System;
using Capibara.Services;

using Foundation;

namespace Capibara.iOS.Services
{
    public class ApplicationService : IApplicationService
    {
        void IApplicationService.Exit() => System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();

        string IApplicationService.StoreUrl => string.Empty;

        string IApplicationService.Platform => "iOS";

        string IApplicationService.AppVersion => NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"].ToString();
    }
}
