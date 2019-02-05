using Unity;

namespace Capibara
{
    public static class ObjectExtensions
    {
        public static T BuildUp<T>(this T source, IUnityContainer container) 
            where T : class
            => source == null ? source : container.BuildUp(source);

        public static bool IsNull<T>(this T source) where T : class
            => source == null;

        public static bool IsPresent<T>(this T source) where T : class
            => !source.IsNull();

        public static int ToInt<T>(this T source, int defaultValue = 0)
        {
            return 
                int.TryParse(source?.ToString() ?? string.Empty, out int result)
                   ? result
                   : defaultValue;
        }
    }
}
