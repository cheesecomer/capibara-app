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

        public override IUnityContainer GenerateUnityContainer()
        {
            var container = base.GenerateUnityContainer();

            var progressDialogService = new Mock<IProgressDialogService>();
            progressDialogService
                .Setup(x => x.DisplayAlertAsync(It.IsAny<Task>(), It.IsAny<string>()))
                .Returns<Task, string>((task, message) => {
                    this.IsDisplayedProgressDialog = true;
                    return task;
                });

            container.RegisterInstance<IProgressDialogService>(progressDialogService.Object);

            return container;
        }
    }
}
