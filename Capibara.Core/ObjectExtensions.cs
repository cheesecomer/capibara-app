using System;

using Unity;

namespace Capibara
{
    public static class ObjectExtensions
    {
        public static T BuildUp<T>(this T source, IUnityContainer container)
            => container.BuildUp(source);

        public static bool IsNull<T>(this T source) where T : class
            => source == null;

        public static bool IsPresent<T>(this T source) where T : class
            => !source.IsNull();

        public static int ToInt<T>(this T source, int defaultValue = 0)
        {
            int result;
            return 
                int.TryParse(source?.ToString() ?? string.Empty, out result)
                   ? result
                   : defaultValue;
        }
    }
}
