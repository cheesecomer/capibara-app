using System;
using System.Linq;

namespace Capibara
{
    public static class StringExtensions
    {
        public static string ToSlim(this string origin) =>
            new [] { " ", "\r", "\n" }
                .Aggregate(
                    origin,
                    (x, v) => x.IsNullOrEmpty()
                        ? string.Empty
                        : x.Replace(v, string.Empty));

        public static bool IsNullOrEmpty(this string origin)
            => string.IsNullOrEmpty(origin);

        public static bool IsPresent(this string origin)
            => !origin.IsNullOrEmpty();
    }
}
