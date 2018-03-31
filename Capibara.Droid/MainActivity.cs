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
    public class MainActivity : FormsApplicationActivity
    {
        public enum RequestCodes
        {
            PickupPhoto = 1,
            CropPhoto = 2
        }

        [Dependency]
        public IIsolatedStorage IsolatedStorage { get; set; }

        internal static MainActivity Instance { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            MainActivity.Instance = this;

            IApplicationService applicationService = new ApplicationService();

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

            var application = new App(new AndroidInitializer(applicationService));

            this.BuildUp(application.Container.Resolve<IUnityContainer>());

            LoadApplication(application);

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

            var intent = this.Intent;

            var uri = intent.Data;
            if (Intent.ActionView.Equals(intent.Action))
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
                }
            }
        }

        protected override async void OnActivityResult(int requestCode, Result resultCode, global::Android.Content.Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok && requestCode == (int)RequestCodes.PickupPhoto)
            {
                this.StartActivityForResult(ImageCropActivity.GetIntent(this.ApplicationContext, data.Data), (int)RequestCodes.CropPhoto);
            }
            else if (resultCode == Result.Ok && requestCode == (int)RequestCodes.CropPhoto)
            {
                using (var bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(this.ContentResolver, data.Data))
                using (var memory = new MemoryStream())
                {
                    await bitmap.CompressAsync(Bitmap.CompressFormat.Png, 100, memory);

                    PickupPhotoService.ActiveTaskCompletionSource.SetResult(memory.ToArray());
                }
            }
            else if (requestCode == (int)RequestCodes.PickupPhoto && requestCode == (int)RequestCodes.CropPhoto)
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
            containerRegistry.RegisterInstance(this.applicationService);
            containerRegistry.RegisterInstance(Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker);
        }
    }
}
