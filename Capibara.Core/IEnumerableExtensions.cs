using System;
using System.Collections.Generic;
namespace Capibara
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            source.ForEach((i, x) => action(x));
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<int, T> action)
        {
            if (source == null) return;

            var index = 0;
            foreach (T element in source)
            {
                action(index, element);
                index++;
            }
        }
    }
}
