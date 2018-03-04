using System;
using System.IO;
namespace Capibara
{
    public static class StreamExtensions
    {
        public static T CopyFrom<T>(this T source, Stream origin) where T : Stream
        {
            origin.CopyTo(source);
            return source;
        }
    }
}
