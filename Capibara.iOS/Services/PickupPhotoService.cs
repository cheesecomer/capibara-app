using System;
using System.IO;
using System.Threading.Tasks;

using Capibara.Services;

using UIKit;

namespace Capibara.iOS.Services
{
    public class PickupPhotoService : IPickupPhotoService
    {
        async Task<byte[]> IPickupPhotoService.DisplayAlbumAsync()
        {
            var taskSource = new TaskCompletionSource<byte[]>();
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

        private EventHandler<UIImagePickerMediaPickedEventArgs> OnFinishedPickingMedia(TaskCompletionSource<byte[]> taskSource)
        {
            return async (sender, args) => {
                //Console.WriteLine(args.Info[UIImagePickerController.ImageUrl]);
                var image = args.Info[UIImagePickerController.OriginalImage] as UIImage;
                await (sender as UIImagePickerController).DismissViewControllerAsync(true);
                using (var memoryStream = new MemoryStream())
                {
                    (args.Info[UIImagePickerController.OriginalImage] as UIImage).AsPNG().AsStream().CopyTo(memoryStream);
                    taskSource.SetResult(memoryStream.ToArray());
                }
                image.Dispose();
            };
        }

        private EventHandler OnCancel(TaskCompletionSource<byte[]> taskSource)
        {
            return async (sender, args) => {
                await (sender as UIImagePickerController).DismissViewControllerAsync(true);
                taskSource.SetResult(null);
            };
        }
    }
}
