using System.IO;
using System.Threading.Tasks;

namespace Capibara.Services
{
    public enum CropMode
    {
        Free,
        Square,
    }

    public interface IPickupPhotoService
    {
        Task<byte[]> DisplayAlbumAsync(CropMode cropMode);
    }
}
