using System;
using System.IO;
using System.Threading.Tasks;

using Capibara.Services;

using UIKit;

using RSKImageCropper;

namespace Capibara.iOS.Services
{
    public class PickupPhotoService : IPickupPhotoService
    {
        async Task<byte[]> IPickupPhotoService.DisplayAlbumAsync()
        {
            var taskSource = new TaskCompletionSource<byte[]>();

            var imagePickerController = new UIImagePickerController();
            imagePickerController.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            imagePickerController.ModalPresentationStyle = UIModalPresentationStyle.Popover;
            imagePickerController.MediaTypes = new [] { "public.image" };
            imagePickerController.FinishedPickingMedia += this.OnFinishedPickingMedia(taskSource);
            imagePickerController.Canceled += this.OnCancel(taskSource);

            var viewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
            while (viewController.PresentedViewController != null)
            {
                viewController = viewController.PresentedViewController;
            }

            await viewController.PresentViewControllerAsync(imagePickerController, true);
            return await taskSource.Task;
        }

        private EventHandler<UIImagePickerMediaPickedEventArgs> OnFinishedPickingMedia(TaskCompletionSource<byte[]> taskSource)
        {
            return async (sender, args) => {
                await (sender as UIViewController).DismissViewControllerAsync(true);

                var cropViewController = new RSKImageCropViewController(args.Info[UIImagePickerController.OriginalImage] as UIImage);
                cropViewController.Canceled += this.OnCancel(taskSource);
                cropViewController.Cropped += this.OnCropped(taskSource);

                var viewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
                while (viewController.PresentedViewController != null)
                {
                    viewController = viewController.PresentedViewController;
                }

                await viewController.PresentViewControllerAsync(cropViewController, true);
            };
        }

        private EventHandler<ImageCropViewControllerCroppedEventArgs> OnCropped(TaskCompletionSource<byte[]> taskSource)
        {
            return async (sender, args) => {
                //Console.WriteLine(args.Info[UIImagePickerController.ImageUrl]);
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
