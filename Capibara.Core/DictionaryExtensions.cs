using System;
using System.Collections.Generic;

namespace Capibara
{
    public static class DictionaryExtensions
    {
        public static TValue ValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue defaultValue = default(TValue))
        {
            return source.TryGetValue(key, out TValue value) ? value : defaultValue;
        }
    }
}
