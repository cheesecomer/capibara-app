using System;
using System.Threading.Tasks;

using Capibara.Services;

using Android.Content;

namespace Capibara.Droid.Services
{
    public class PickupPhotoService : IPickupPhotoService
    {
        public static TaskCompletionSource<byte[]> ActiveTaskCompletionSource { get; set; }

        Task<byte[]> IPickupPhotoService.DisplayAlbumAsync(CropMode cropMode)
        {
            var requestCode = cropMode == CropMode.Free 
                ? MainActivity.RequestCodes.PickupPhotoForFree 
                : MainActivity.RequestCodes.PickupPhotoForSquare;
            var imageIntent = new Intent();
            imageIntent.SetType("image/*");
            imageIntent.SetAction(Intent.ActionGetContent);
            MainActivity.Instance.StartActivityForResult(
                Intent.CreateChooser(imageIntent, "Select photo"),
                (int)requestCode);

            return (ActiveTaskCompletionSource = new TaskCompletionSource<byte[]>()).Task;
        }
    }
}
