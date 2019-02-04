#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System.Threading.Tasks;

using Moq;

using Prism.Behaviors;
using Prism.Common;
using Prism.Ioc;
using Prism.Logging;
using Prism.Navigation;

namespace Capibara.Presentation.ViewModels
{
    public abstract class NavigationService : PageNavigationService
    {
        protected NavigationService()
            : base(
                new Mock<IContainerExtension>().Object,
                new Mock<IApplicationProvider>().Object,
                new Mock<IPageBehaviorFactory>().Object,
                new Mock<ILoggerFacade>().Object) { }

        public abstract Task<INavigationResult> NavigateAsync(string name, INavigationParameters parameters, bool? useModalNavigation, bool animated);

        protected override Task<INavigationResult> NavigateInternal(string name, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            return this.NavigateAsync(name, parameters, useModalNavigation, animated);
        }
    }
}
