using System;
using System.IO;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;

using Capibara.Droid.Services;
using Capibara.Services;

using Plugin.GoogleAnalytics;

using Prism;
using Prism.Ioc;

using Unity;
using Unity.Attributes;

using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace Capibara.Droid
{
    [Activity(Label = "Capibara", Icon = "@drawable/icon", Theme = "@style/Theme.Main", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataScheme = "com.cheesecomer.capibara", DataHost = "oauth")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public enum RequestCodes
        {
            PickupPhotoForSquare = 1,
            PickupPhotoForFree = 2,
            CropPhoto = 3
        }

        [Dependency]
        public IIsolatedStorage IsolatedStorage { get; set; }

        internal static MainActivity Instance { get; private set; }

        internal App App;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            MainActivity.Instance = this;

            IApplicationService applicationService = new ApplicationService();

            Android.Gms.Ads.MobileAds.Initialize(this, PlatformVariable.AdMobApplicationId);

            GoogleAnalytics.Current.Config.TrackingId = PlatformVariable.GoogleAnalyticsTrackingId;
            GoogleAnalytics.Current.Config.AppId = "Capibara";
            GoogleAnalytics.Current.Config.AppName = "Capibara";
            GoogleAnalytics.Current.Config.AppVersion = applicationService.AppVersion;
            GoogleAnalytics.Current.Config.AutoAppLifetimeMonitoring = true;
            GoogleAnalytics.Current.Config.ReportUncaughtExceptions = true;
            GoogleAnalytics.Current.InitTracker();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                this.Window.DecorView.SystemUiVisibility = 0;
                var statusBarHeightInfo = 
                    typeof(FormsApplicationActivity)
                        .GetField("_statusBarHeight", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                statusBarHeightInfo?.SetValue(this, 0);
                this.Window.SetStatusBarColor(new Color(0x58, 0xCE, 0x91, 255));
            }

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            this.App = new App(new AndroidInitializer(applicationService));

            this.BuildUp(this.App.Container.Resolve<IUnityContainer>());

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            LoadApplication(this.App);

            Xamarin
                .Forms
                .Application
                .Current
                .On<Xamarin.Forms.PlatformConfiguration.Android>()
                .UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (this.Intent == null) return;

            var uri = this.Intent.Data;
            if (Intent.ActionView.Equals(this.Intent.Action))
            {
                if (uri.Scheme == "com.cheesecomer.capibara" && uri.Host.StartsWith("oauth", StringComparison.Ordinal))
                {
                    var query = uri.Query
                       .Replace("?", string.Empty).Split('&')
                       .Select(x => x.Split('='))
                       .Where(x => x.Length == 2)
                       .ToDictionary(x => x.First(), x => x.Last());
                    this.IsolatedStorage.AccessToken = query["access_token"];
                    this.IsolatedStorage.UserId = query["id"].ToInt();
                    this.IsolatedStorage.Save();
                }
            }

            this.Intent = null;
        }

        protected override void OnPause()
        {
            this.App.Sleep();

            base.OnPause();
        }

        protected override async void OnActivityResult(int requestCode, Result resultCode, global::Android.Content.Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok && requestCode == (int)RequestCodes.PickupPhotoForFree)
            {
                this.StartActivityForResult(ImageCropActivity.GetIntent(this.ApplicationContext, data.Data, false), (int)RequestCodes.CropPhoto);
            }
            else if (resultCode == Result.Ok && requestCode == (int)RequestCodes.PickupPhotoForSquare)
            {
                this.StartActivityForResult(ImageCropActivity.GetIntent(this.ApplicationContext, data.Data, true), (int)RequestCodes.CropPhoto);
            }
            else if (resultCode == Result.Ok && requestCode == (int)RequestCodes.CropPhoto)
            {
                using (var bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(this.ContentResolver, data.Data))
                using (var memory = new MemoryStream())
                {
                    await bitmap.CompressAsync(Bitmap.CompressFormat.Png, 100, memory);

                    PickupPhotoService.ActiveTaskCompletionSource.SetResult(memory.ToArray());
                }

                new Java.IO.File(data.Data.Path).Delete();
            }
            else if (requestCode == (int)RequestCodes.PickupPhotoForSquare && requestCode == (int)RequestCodes.PickupPhotoForFree && requestCode == (int)RequestCodes.CropPhoto)
            {
                PickupPhotoService.ActiveTaskCompletionSource.SetResult(null);
            }
        }
    }

    public class AndroidInitializer : IPlatformInitializer
    {
        private IApplicationService applicationService;

        public AndroidInitializer(IApplicationService applicationService)
        {
            this.applicationService = applicationService;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<IIsolatedStorage>(new IsolatedStorage());
            containerRegistry.RegisterInstance<IProgressDialogService>(new ProgressDialogService());
            containerRegistry.RegisterInstance<IPickupPhotoService>(new PickupPhotoService());
            containerRegistry.RegisterInstance<IScreenService>(new ScreenService());
            containerRegistry.RegisterInstance<IBalloonService>(new BalloonService());
            containerRegistry.RegisterInstance<ISnsLoginService>(new SnsLoginService());
            containerRegistry.RegisterInstance<IRewardedVideoService>(new RewardedVideoService());
            containerRegistry.RegisterInstance(this.applicationService);
            containerRegistry.RegisterInstance(GoogleAnalytics.Current.Tracker);
        }
    }
}
