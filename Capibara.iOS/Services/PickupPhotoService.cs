using System;
using System.IO;
using System.Threading.Tasks;

using Capibara.Services;

using CoreGraphics;
using UIKit;
using Foundation;

using Microsoft.Practices.Unity;

namespace Capibara.iOS.Services
{
    public class PickupPhotoService : IPickupPhotoService
    {
        async Task<Stream> IPickupPhotoService.DisplayAlbumAsync()
        {
            var taskSource = new TaskCompletionSource<Stream>();
            await Task.Delay(100);

            var imagePickerController = new UIImagePickerController();
            imagePickerController.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            imagePickerController.ModalPresentationStyle = UIModalPresentationStyle.Popover;
            imagePickerController.MediaTypes = new [] { "public.image" };
            imagePickerController.FinishedPickingMedia += this.OnFinishedPickingMedia(taskSource);
            imagePickerController.Canceled += this.OnCancel(taskSource);

            var viewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
            await viewController.PresentViewControllerAsync(imagePickerController, true);
            return await taskSource.Task;
        }

        private EventHandler<UIImagePickerMediaPickedEventArgs> OnFinishedPickingMedia(TaskCompletionSource<Stream> taskSource)
        {
            return async (sender, args) => {
                //Console.WriteLine(args.Info[UIImagePickerController.ImageUrl]);
                var image = args.Info[UIImagePickerController.OriginalImage] as UIImage;
                await (sender as UIImagePickerController).DismissViewControllerAsync(true);
                taskSource.SetResult((args.Info[UIImagePickerController.OriginalImage] as UIImage).AsPNG().AsStream());
                image.Dispose();
            };
        }

        private EventHandler OnCancel(TaskCompletionSource<Stream> taskSource)
        {
            return async (sender, args) => {
                await (sender as UIImagePickerController).DismissViewControllerAsync(true);
                taskSource.SetResult(null);
            };
        }
    }
}
