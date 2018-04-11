using Prism.Ioc;
using Unity;

namespace Capibara
{
    public static class IContainerProviderExtensions
    {
        public static T TryResolve<T>(this IContainerProvider provider)
        {
            try
            {
                return provider.Resolve<T>();
            }
            catch
            {
                return default(T);
            }
        }
    }
}
