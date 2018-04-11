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
        protected Mock<NavigationService> NavigationService { get; private set; }

        protected Mock<IPageDialogService> PageDialogService { get; private set; }

        protected Mock<IDeviceService> DeviceService { get; private set; }

        protected Mock<IProgressDialogService> ProgressDialogService { get; set; }

        protected Mock<IPickupPhotoService> PickupPhotoService { get; set; }

        protected Mock<ISnsLoginService> SnsLoginService { get; set; }

        protected Mock<IRewardedVideoService> RewardedVideoService { get; set; }

        protected Mock<Plugin.GoogleAnalytics.Abstractions.ITracker> Tracker { get; set; }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.PageDialogService = new Mock<IPageDialogService>();
            this.PageDialogService
                .Setup(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            
            this.PageDialogService
                .Setup(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            
            this.NavigationService = new Mock<NavigationService> { CallBase = true };
            this.NavigationService
                .Setup(x => x.NavigateAsync(It.IsAny<string>(), It.IsAny<NavigationParameters>(), It.IsAny<bool?>(), It.IsAny<bool>()))
                .Returns(Task.CompletedTask);

            this.NavigationService
                .Setup(x => x.GoBackAsync())
                .ReturnsAsync(true);

            this.ProgressDialogService = new Mock<IProgressDialogService>();
            this.ProgressDialogService
                .Setup(x => x.DisplayProgressAsync(It.IsAny<Task>(), It.IsAny<string>()))
                .Returns<Task, string>((task, message) => task);

            // IPickupPhotoService のセットアップ
            this.PickupPhotoService = new Mock<IPickupPhotoService>();
            this.PickupPhotoService.SetupAllProperties();
            this.PickupPhotoService
                .Setup(x => x.DisplayAlbumAsync())
                .Returns(Task.FromResult(new byte[0]));

            this.DeviceService = new Mock<IDeviceService>();
            this.DeviceService.Setup(x => x.BeginInvokeOnMainThread(It.IsAny<Action>())).Callback((Action action) => action?.Invoke());
            this.Container.RegisterInstance(this.DeviceService.Object);

            this.SnsLoginService = new Mock<ISnsLoginService>();
            this.SnsLoginService.Setup(x => x.Open(It.IsAny<string>()));

            this.RewardedVideoService = new Mock<IRewardedVideoService>();

            var taskService = new Mock<ITaskService>();
            taskService.Setup(x => x.Delay(It.IsAny<int>())).Returns(Task.CompletedTask);

            this.Tracker = new Mock<Plugin.GoogleAnalytics.Abstractions.ITracker>();

            var balloonService = new Mock<IBalloonService>();

            this.Container.RegisterInstance(this.ProgressDialogService.Object);
            this.Container.RegisterInstance(this.PickupPhotoService.Object);
            this.Container.RegisterInstance(this.SnsLoginService.Object);
            this.Container.RegisterInstance(this.RewardedVideoService.Object);
            this.Container.RegisterInstance(taskService.Object);
            this.Container.RegisterInstance(this.Tracker.Object);
            this.Container.RegisterInstance(balloonService.Object);
        }
    }
}
