using System;
using System.Threading.Tasks;

using Moq;

using Prism.Behaviors;
using Prism.Common;
using Prism.Ioc;
using Prism.Logging;
using Prism.Navigation;

namespace Capibara.Test
{
    public abstract class NavigationService : PageNavigationService
    {
        protected NavigationService()
            : base(
                new Mock<IContainerExtension>().Object,
                new Mock<IApplicationProvider>().Object,
                new Mock<IPageBehaviorFactory>().Object,
                new Mock<ILoggerFacade>().Object)
        {
        }

        public abstract Task<INavigationResult> NavigateAsync(string name, INavigationParameters parameters, bool? useModalNavigation, bool animated);

        protected override Task<INavigationResult> NavigateInternal(string name, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            return this.NavigateAsync(name, parameters, useModalNavigation, animated);
        }
    }

    public static class NavigationParametersExtension
    {
        public static object GetValueOrDefault(this NavigationParameters source, string key)
        {
            return source.ContainsKey(key) ? source[key] : null;
        }
    }
}
