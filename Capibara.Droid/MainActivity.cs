using System.IO;

using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;

using Capibara.Droid.Services;
using Capibara.Services;

using Prism;
using Prism.Ioc;

using Unity;
using Unity.Attributes;

namespace Capibara.Droid
{
    [Activity(Label = "Capibara", Icon = "@drawable/icon", MainLauncher = true, Theme = "@android:style/Theme.Holo.Light", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
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

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            var application = new App(new AndroidInitializer(applicationService));

            this.BuildUp(application.Container.Resolve<IUnityContainer>());

            LoadApplication(application);
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
