using System;
using System.Threading.Tasks;

using Capibara.Services;

using Microsoft.Practices.Unity;
using Moq;
namespace Capibara.Test.ViewModels
{
    public abstract class ViewModelTestBase : TestFixtureBase
    {
        public override IUnityContainer GenerateUnityContainer()
        {
            var container = base.GenerateUnityContainer();

            var progressDialogService = new Mock<IProgressDialogService>();
            progressDialogService.Setup(x => x.DisplayAlertAsync(It.IsAny<Task>(), It.IsAny<string>())).Returns<Task, string>((task, message) => task);

            container.RegisterInstance<IProgressDialogService>(progressDialogService.Object);

            return container;
        }
    }
}
