#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System.Threading.Tasks;
using Moq;
using Prism.Navigation;
using Prism.Services;

namespace Capibara.Presentation.ViewModels
{
    public static class Mock
    {
        public static Mock<NavigationService> NavigationService()
        {
            var navigationService = new Mock<NavigationService> { CallBase = true };
            navigationService
                .Setup(x => x.NavigateAsync(It.IsAny<string>(), It.IsAny<INavigationParameters>(), It.IsAny<bool?>(), It.IsAny<bool>()))
                .ReturnsAsync(new NavigationResult());

            return navigationService;
        }

        public static Mock<IPageDialogService> PageDialogService(bool shouldRetry = false)
        {
            var pageDialogService = new Mock<IPageDialogService>();
            pageDialogService
                .Setup(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            pageDialogService
                .Setup(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(shouldRetry);

            return pageDialogService;
        }
    }
}
