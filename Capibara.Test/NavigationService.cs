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
        public NavigationService()
            : base(
                new Mock<IContainerExtension>().Object,
                new Mock<IApplicationProvider>().Object,
                new Mock<IPageBehaviorFactory>().Object,
                new Mock<ILoggerFacade>().Object)
        {
        }

        public abstract Task NavigateAsync(string name, NavigationParameters parameters, bool? useModalNavigation, bool animated);

        protected override Task NavigateInternal(string name, NavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            return this.NavigateAsync(name, parameters, useModalNavigation, animated);
        }
    }
}
