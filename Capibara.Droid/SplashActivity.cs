
using Android.App;
using Android.Content;
using Android.Content.PM;

namespace Capibara.Droid
{
    [Activity(Label = "Capibara", NoHistory = true, Icon = "@drawable/icon", MainLauncher = true, Theme = "@style/Theme.Splash", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashActivity : Activity
    {
        protected override void OnResume()
        {
            base.OnResume();

            this.StartActivity(new Intent(this, typeof(MainActivity)));
        }
    }
}
