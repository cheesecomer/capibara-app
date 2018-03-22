using Prism;
using Prism.AppModel;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation;
using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Capibara.Ioc
{
    public class ContainerProvider<T>
    {
        /// <summary>
        /// The Name used to register the type with the Container
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Resolves the specified type from the Application's Container
        /// </summary>
        /// <param name="containerProvider"></param>
        public static implicit operator T(ContainerProvider<T> containerProvider)
        {
            var container = (Application.Current as PrismApplicationBase).Container;
            if (container == null) return default(T);
            if (string.IsNullOrWhiteSpace(containerProvider.Name))
            {
                return container.Resolve<T>();
            }

            return container.Resolve<T>(containerProvider.Name);
        }
    }
}
