using System;
using System.IO;

using Xamarin.Forms;

namespace Capibara.Forms
{
    public interface IImageSourceFactory
    {
        ImageSource FromUri(Uri uri);

        ImageSource FromStream(Func<Stream> func);
    }

    public class ImageSourceFactory : IImageSourceFactory
    {
        ImageSource IImageSourceFactory.FromUri(Uri uri) => ImageSource.FromUri(uri);

        ImageSource IImageSourceFactory.FromStream(Func<Stream> func) => ImageSource.FromStream(func);
    }
}
