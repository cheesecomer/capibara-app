using System;
using Prism.Navigation;

namespace Capibara.Presentation
{
    public static class NavigationParametersExtensions
    {
        public static T TryGetValue<T>(this INavigationParameters args, string key)
        {
            return args.TryGetValue(key, out T value) ? value : default;
        }
    }
}
