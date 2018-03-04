using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

using Xamarin.Forms;
namespace Capibara
{
    public static class ImageSourceExtensions
    {
        public static byte[] ToByteArray(this ImageSource imageSource)
        {
            var streamImageSource = imageSource as StreamImageSource;
            var stream = streamImageSource.IsPresent() ? streamImageSource.Stream(CancellationToken.None).Result : null;
            using (var memoryStream = new MemoryStream())
            {
                stream?.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
