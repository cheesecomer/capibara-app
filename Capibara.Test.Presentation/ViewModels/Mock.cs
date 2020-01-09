#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Capibara.Services;
using Moq;
using Moq.Language;
using Moq.Language.Flow;
using Prism.Navigation;
using Prism.Services;

namespace Capibara.Presentation.ViewModels
{
    public static class Mock
    {
        public static IReturnsResult<TMock> ReturnsObservable<TMock>(this IReturns<TMock, IObservable<Unit>> mock)
            where TMock : class =>
            mock.Returns(Observable.Return(Unit.Default));

        public static IReturnsResult<TMock> ReturnsObservable<TMock, TResult>(this IReturns<TMock, IObservable<TResult>> mock, TResult value)
            where TMock : class =>
            mock.Returns(Observable.Return(value));

        public static IReturnsResult<TMock> ReturnsObservable<TMock, TResult>(this IReturns<TMock, IObservable<TResult>> mock, Exception value)
            where TMock : class =>
            mock.Returns(Observable.Throw<TResult>(value));

        public static Mock<NavigationService> NavigationService()
        {
            var navigationService = new Mock<NavigationService> { CallBase = true };
            navigationService
                .Setup(x => x.NavigateAsync(It.IsAny<string>(), It.IsAny<INavigationParameters>(), It.IsAny<bool?>(), It.IsAny<bool>()))
                .ReturnsAsync(new NavigationResult());

            return navigationService;
        }

        public static Mock<IPageDialogService> PageDialogService(bool? shouldRetry = false)
        {
            var pageDialogService = new Mock<IPageDialogService>();
            pageDialogService
                .Setup(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var alert = pageDialogService
                .Setup(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            if (shouldRetry.HasValue)
            {
                alert.ReturnsAsync(shouldRetry.Value);
            }
            else
            {
                alert.Returns(Observable.Never<bool>().ToTask());
            }

            return pageDialogService;
        }

        public static Mock<IProgressDialogService> ProgressDialogService<T>()
        {
            var progressDialogService = new Mock<IProgressDialogService>();

            progressDialogService
                .Setup(x => x.DisplayProgressAsync(It.IsAny<IObservable<T>>(), It.IsAny<string>()))
                .Returns((IObservable<T> observable, string _) => observable);

            progressDialogService
                .Setup(x => x.DisplayProgressAsync(It.IsAny<Task>(), It.IsAny<string>()))
                .Returns((Task task, string _) => task);

            progressDialogService
                .Setup(x => x.DisplayProgressAsync(It.IsAny<Task<T>>(), It.IsAny<string>()))
                .Returns((Task<T> task, string _) => task);

            return progressDialogService;
        }

        public static Mock<IProgressDialogService> ProgressDialogService()
        {
            var progressDialogService = new Mock<IProgressDialogService>();

            progressDialogService
                .Setup(x => x.DisplayProgressAsync(It.IsAny<Task>(), It.IsAny<string>()))
                .Returns((Task task, string _) => task);

            return progressDialogService;
        }
    }
}
