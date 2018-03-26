using System;

using Capibara.Services;

namespace Capibara.Droid.Services
{
    public class ApplicationService : IApplicationService
    {
        string IApplicationService.StoreUrl => string.Empty;

        string IApplicationService.Platform => "Android";

        string IApplicationService.AppVersion
        {
            get
            {
                var context = MainActivity.Instance.ApplicationContext;
                return context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;
            }
        }

        void IApplicationService.Exit()
            => System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
    }
}
