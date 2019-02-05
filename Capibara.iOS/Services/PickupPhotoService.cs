using System;
using System.IO;
using System.Threading.Tasks;
using Capibara.Services;
using PhotoTweaks;
using RSKImageCropper;
using UIKit;

namespace Capibara.iOS.Services
{
    public class PickupPhotoService : IPickupPhotoService
    {
        async Task<byte[]> IPickupPhotoService.DisplayAlbumAsync(CropMode cropMode)
        {
            var taskSource = new TaskCompletionSource<byte[]>();

            var imagePickerController = new UIImagePickerController
            {
                SourceType = UIImagePickerControllerSourceType.PhotoLibrary,
                MediaTypes = new [] { "public.image" }
            };

            imagePickerController.FinishedPickingMedia += this.OnFinishedPickingMedia(taskSource, cropMode);
            imagePickerController.Canceled += this.OnCancel(taskSource);

            var viewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
            while (viewController.PresentedViewController != null)
            {
                viewController = viewController.PresentedViewController;
            }

            await viewController.PresentViewControllerAsync(imagePickerController, true);
            return await taskSource.Task;
        }

        private EventHandler<UIImagePickerMediaPickedEventArgs> OnFinishedPickingMedia(TaskCompletionSource<byte[]> taskSource, CropMode cropMode)
        {
            return async (sender, args) => {
                await (sender as UIViewController).DismissViewControllerAsync(true);

                UIViewController nextViewController = null;

                if (cropMode == CropMode.Free)
                {
                    var cropViewController = new PhotoTweaksViewController(args.Info[UIImagePickerController.OriginalImage] as UIImage);
                    cropViewController.Cropped += this.OnCroppedFree(taskSource);
                    cropViewController.Canceled += this.OnCancel(taskSource);
                    cropViewController.AutoSaveToLibray = false;

                    nextViewController = cropViewController;
                }
                else if (cropMode == CropMode.Square)
                {
                    var cropViewController = new RSKImageCropViewController(args.Info[UIImagePickerController.OriginalImage] as UIImage);
                    cropViewController.Cropped += this.OnCroppedSquare(taskSource);
                    cropViewController.Canceled += this.OnCancel(taskSource);

                    nextViewController = cropViewController;
                }


                var viewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
                while (viewController.PresentedViewController != null)
                {
                    viewController = viewController.PresentedViewController;
                }

                await viewController.PresentViewControllerAsync(nextViewController, true);
            };
        }

        private EventHandler<ImageCropViewControllerCroppedEventArgs> OnCroppedSquare(TaskCompletionSource<byte[]> taskSource)
        {
            return async (sender, args) => {
                var image = args.CroppedImage;
                await (sender as UIViewController).DismissViewControllerAsync(true);
                using (var memoryStream = new MemoryStream())
                {
                    image.AsPNG().AsStream().CopyTo(memoryStream);
                    taskSource.SetResult(memoryStream.ToArray());
                }
                image.Dispose();
            };
        }

        private EventHandler<PhotoTweaksViewControllerCroppedEventArgs> OnCroppedFree(TaskCompletionSource<byte[]> taskSource)
        {
            return async (sender, args) => {
                var image = args.CroppedImage;
                await (sender as UIViewController).DismissViewControllerAsync(true);
                using (var memoryStream = new MemoryStream())
                {
                    image.AsPNG().AsStream().CopyTo(memoryStream);
                    taskSource.SetResult(memoryStream.ToArray());
                }
                image.Dispose();
            };
        }

        private EventHandler OnCancel(TaskCompletionSource<byte[]> taskSource)
        {
            return async (sender, args) => {
                await (sender as UIViewController).DismissViewControllerAsync(true);
                taskSource.SetResult(null);
            };
        }
    }
}
