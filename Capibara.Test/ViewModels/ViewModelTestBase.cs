using System;
using System.Threading.Tasks;

using Capibara.Services;

using Unity;
using Moq;

using Prism.Navigation;

using NUnit.Framework;
namespace Capibara.Test.ViewModels
{
    public abstract class ViewModelTestBase : TestFixtureBase
    {
        protected string NavigatePageName { get; private set; }

        protected NavigationParameters NavigationParameters { get; private set; }

        protected INavigationService NavigationService { get; private set; }

        protected bool IsDisplayedProgressDialog { get; private set; }

        protected bool IsDisplayedPhotoPicker { get; private set; }

        [SetUp]
        public void Initialize()
        {
            var navigateTaskSource = new TaskCompletionSource<bool>();
            var navigationServiceMock = new Mock<NavigationService> { CallBase = true };
            navigationServiceMock
                .Setup(x => x.NavigateAsync(It.IsAny<string>(), It.IsAny<NavigationParameters>(), It.IsAny<bool?>(), It.IsAny<bool>()))
                .Returns(navigateTaskSource.Task)
                .Callback((string name, NavigationParameters parameters, bool? useModalNavigation, bool animated) =>
                {
                    this.NavigatePageName = name;
                    this.NavigationParameters = parameters;
                    navigateTaskSource.SetResult(true);
                });

            this.NavigationService = navigationServiceMock.Object;
        }

        public override IUnityContainer GenerateUnityContainer()
        {
            var container = base.GenerateUnityContainer();

            var progressDialogService = new Mock<IProgressDialogService>();
            progressDialogService
                .Setup(x => x.DisplayProgressAsync(It.IsAny<Task>(), It.IsAny<string>()))
                .Returns<Task, string>((task, message) => {
                    this.IsDisplayedProgressDialog = true;
                    return task;
                });

            container.RegisterInstance<IProgressDialogService>(progressDialogService.Object);

            // IPickupPhotoService のセットアップ
            var pickupPhotoService = new Mock<IPickupPhotoService>();
            pickupPhotoService.SetupAllProperties();
            pickupPhotoService
                .Setup(x => x.DisplayAlbumAsync())
                .Returns(() => {
                    var taskSource = new TaskCompletionSource<byte[]>();
                    taskSource.SetResult(new byte[0]);
                    this.IsDisplayedPhotoPicker = true;
                    return taskSource.Task;
                });

            container.RegisterInstance<IPickupPhotoService>(pickupPhotoService.Object);

            return container;
        }
    }
}
