using System;

using Prism.Navigation;
namespace Capibara
{
    public static class NavigationParametersExtensions
    {
        public static T TryGetValue<T>(this NavigationParameters args, string key)
        {
            T value = default(T);
            return args.TryGetValue(key, out value) ? value : default(T);
        }
    }
}
