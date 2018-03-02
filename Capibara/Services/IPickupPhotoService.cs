using System.IO;
using System.Threading.Tasks;

namespace Capibara.Services
{
    public interface IPickupPhotoService
    {
        Task<Stream> DisplayAlbumAsync();
    }
}
