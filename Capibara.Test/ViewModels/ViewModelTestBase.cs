using System;
using System.Threading.Tasks;

using Capibara.Services;

using Unity;
using Moq;

using Prism.Services;
using Prism.Navigation;

using NUnit.Framework;
namespace Capibara.Test.ViewModels
{
    public abstract class ViewModelTestBase : TestFixtureBase
    {
        protected string NavigatePageName { get; private set; }

        protected NavigationParameters NavigationParameters { get; private set; }

        protected INavigationService NavigationService { get; private set; }

        protected Mock<IDeviceService> DeviceService { get; private set; }

        protected bool IsDisplayedProgressDialog { get; private set; }

        protected bool IsDisplayedPhotoPicker { get; private set; }

        protected bool IsGoBackCalled { get; private set; }

        [SetUp]
        public void Initialize()
        {
            var navigationServiceMock = new Mock<NavigationService> { CallBase = true };
            navigationServiceMock
                .Setup(x => x.NavigateAsync(It.IsAny<string>(), It.IsAny<NavigationParameters>(), It.IsAny<bool?>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(true))
                .Callback((string name, NavigationParameters parameters, bool? useModalNavigation, bool animated) =>
                {
                    this.NavigatePageName = name;
                    this.NavigationParameters = parameters;
                });

            navigationServiceMock
                .Setup(x => x.GoBackAsync())
                .Callback(() => this.IsGoBackCalled = true)
                .ReturnsAsync(true);

            this.NavigationService = navigationServiceMock.Object;
        }

        public override IUnityContainer GenerateUnityContainer()
        {
            var container = base.GenerateUnityContainer();

            var progressDialogService = new Mock<IProgressDialogService>();
            progressDialogService
                .Setup(x => x.DisplayProgressAsync(It.IsAny<Task>(), It.IsAny<string>()))
                .Callback<Task, string>((task, message) => this.IsDisplayedProgressDialog = true)
                .Returns<Task, string>((task, message) => task);

            container.RegisterInstance(progressDialogService.Object);

            // IPickupPhotoService のセットアップ
            var pickupPhotoService = new Mock<IPickupPhotoService>();
            pickupPhotoService.SetupAllProperties();
            pickupPhotoService
                .Setup(x => x.DisplayAlbumAsync())
                .Callback(() => this.IsDisplayedPhotoPicker = true)
                .Returns(Task.FromResult(new byte[0]));

            container.RegisterInstance(pickupPhotoService.Object);

            this.DeviceService = new Mock<IDeviceService>();
            container.RegisterInstance(this.DeviceService.Object);

            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.Delay(It.IsAny<int>())).Returns(Task.CompletedTask);

            container.RegisterInstance(taskService.Object);

            return container;
        }
    }
}
