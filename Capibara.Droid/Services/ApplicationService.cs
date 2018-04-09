using Capibara.Services;

namespace Capibara.Droid.Services
{
    public class ApplicationService : IApplicationService
    {
        private string uuid;

        string IApplicationService.StoreUrl => "https://play.google.com/store/apps/details?id=com.cheesecomer.Capibara&hl=ja&ah=hTX9fjpkOefw1O8lhfrlt5IHbyM";

        string IApplicationService.Platform => "Android";

        string IApplicationService.UUID => this.uuid.Presence() ?? (this.uuid = Android.Provider.Settings.Secure.GetString(MainActivity.Instance.ContentResolver, Android.Provider.Settings.Secure.AndroidId));

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
