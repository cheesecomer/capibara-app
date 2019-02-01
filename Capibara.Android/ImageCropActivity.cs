
using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using Com.Isseiaoki.Simplecropview;
using Com.Isseiaoki.Simplecropview.Callback;
using Java.IO;
using AndroidUri = Android.Net.Uri;

namespace Capibara.Droid
{
    [Activity(Label = "ImageCropActivity")]
    public class ImageCropActivity : Activity
    {
        public class Arguments
        {
            public const string Data = "ARGUMENT_DATA";

            public const string IsSquare = "ARGUMENT_IS_SQUARE";
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.activity_image_crop);

            var cropImageView = this.FindViewById<CropImageView>(Resource.Id.crop_image);

            var isSquare = this.Intent.GetBooleanExtra(Arguments.IsSquare, false);
            if (isSquare)
            {
                cropImageView.SetCropMode(CropImageView.CropMode.CircleSquare);
            }
            else
            {
                cropImageView.SetCropMode(CropImageView.CropMode.Free);
            }

            var cropButton = this.FindViewById<Button>(Resource.Id.crop_button);
            cropButton.Click += (s, e) =>
            {
                var filename = DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_cropped.jpg";
                var file = new File(this.ExternalCacheDir, filename);
                file.CreateNewFile();
                file.SetWritable(true);

                var uri = AndroidUri.FromFile(file);
               
                var cropCallback = 
                    new CropCallback()
                        .OnSuccessDo(x => { })
                        .OnErrorDo(() => { });
                var saveCallback = new SaveCallback()
                    .OnSuccessDo(x =>
                    {
                        this.SetResult(Result.Ok, new Intent().SetData(uri));
                        this.Finish();
                    })
                    .OnErrorDo(() => { });

                cropImageView.StartCrop(uri, cropCallback, saveCallback);
            };

            cropButton.Enabled = false;

            var origin = this.Intent.GetParcelableExtra(Arguments.Data) as AndroidUri;

            var loadCallback = new LoadCallback()
                .OnSuccessDo(() => cropButton.Enabled = true)
                .OnErrorDo(() => { });

            cropImageView.StartLoad(origin, loadCallback);
        }

        public static Intent GetIntent(Context context, AndroidUri uri, bool isSquare)
        {
            var intent = new Intent(context, typeof(ImageCropActivity));
            intent.PutExtra(Arguments.Data, uri);
            intent.PutExtra(Arguments.IsSquare, isSquare);

            return intent;
        }
    }

    public class LoadCallback : Java.Lang.Object, ILoadCallback
    {
        Action onError;
        Action onSuccess;

        /// <summary>
        /// Adds the error.
        /// </summary>
        /// <returns>The error.</returns>
        /// <param name="error">Error.</param>
        public LoadCallback OnErrorDo(Action error)
        {
            this.onError = error;
            return this;
        }

        /// <summary>
        /// Adds the success.
        /// </summary>
        /// <returns>The success.</returns>
        /// <param name="success">Success.</param>
        public LoadCallback OnSuccessDo(Action success)
        {
            this.onSuccess = success;
            return this;
        }

        public void OnError()
        {
            this.onError?.Invoke();
        }

        public void OnSuccess()
        {
            this.onSuccess?.Invoke();
        }
    }

    /// <summary>
    /// Crop call back.
    /// </summary>
    public class CropCallback : Java.Lang.Object, ICropCallback
    {
        Action onError;
        Action<Bitmap> onSuccess;

        /// <summary>
        /// Adds the error.
        /// </summary>
        /// <returns>The error.</returns>
        /// <param name="error">Error.</param>
        public CropCallback OnErrorDo(Action error)
        {
            this.onError = error;
            return this;
        }

        /// <summary>
        /// Adds the success.
        /// </summary>
        /// <returns>The success.</returns>
        /// <param name="success">Success.</param>
        public CropCallback OnSuccessDo(Action<Bitmap> success)
        {
            this.onSuccess = success;
            return this;
        }

        public void OnError()
        {
            this.onError?.Invoke();
        }

        public void OnSuccess(Bitmap bitmap)
        {
            this.onSuccess?.Invoke(bitmap);
        }
    }

    /// <summary>
    /// Save call back.
    /// </summary>
    public class SaveCallback : Java.Lang.Object, ISaveCallback
    {
        Action onError;
        Action<AndroidUri> onSuccess;

        /// <summary>
        /// Adds the error.
        /// </summary>
        /// <returns>The error.</returns>
        /// <param name="error">Error.</param>
        public SaveCallback OnErrorDo(Action error)
        {
            this.onError = error;
            return this;
        }

        /// <summary>
        /// Adds the success.
        /// </summary>
        /// <returns>The success.</returns>
        /// <param name="success">Success.</param>
        public SaveCallback OnSuccessDo(Action<AndroidUri> success)
        {
            this.onSuccess = success;
            return this;
        }

        public void OnError()
        {
            this.onError?.Invoke();
        }

        public void OnSuccess(AndroidUri uri)
        {
            this.onSuccess?.Invoke(uri);
        }
    }
}
