using System;
using System.Threading.Tasks;

using Capibara.Services;

using Microsoft.Practices.Unity;
using Moq;
namespace Capibara.Test.ViewModels
{
    public abstract class ViewModelTestBase : TestFixtureBase
    {
        protected bool IsDisplayedProgressDialog { get; private set; }

        protected bool IsDisplayedPhotoPicker { get; private set; }

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
