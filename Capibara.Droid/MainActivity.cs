using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Capibara.Services;
using Capibara.Droid.Services;

using Unity;

using Prism;
using Prism.Ioc;

namespace Capibara.Droid
{
    [Activity(Label = "Capibara.Droid", Icon = "@drawable/icon", MainLauncher = true, Theme = "@android:style/Theme.Holo.Light", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        public enum RequestCodes
        {
            PickupPhoto = 1,
            CropPhoto = 2
        }

        internal static MainActivity Instance { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            MainActivity.Instance = this;

            global::Xamarin.Forms.Forms.Init(this, bundle);

            LoadApplication(new App(new AndroidInitializer()));
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, global::Android.Content.Intent data)
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
                    bitmap.Compress(Bitmap.CompressFormat.Png, 100, memory);

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
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<IScreenService>(new ScreenService());
        }
    }
}
