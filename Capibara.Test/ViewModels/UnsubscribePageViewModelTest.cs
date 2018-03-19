using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;

using Microsoft.Practices.Unity;

using Moq;
using NUnit.Framework;

using Unity;

using Prism.Navigation;

namespace Capibara.Test.ViewModels.UnsubscribePageViewModelTest
{
    public class UnsubscribeCommandTest : ViewModelTestBase
    {
        private bool IsDestroyCalled { get; set; }

        private UnsubscribePageViewModel Actual { get; set; }

        [SetUp]
        public void SetUp()
        {
            var container = this.GenerateUnityContainer();

            var currentUser = new Mock<User>();

            currentUser.Setup(x => x.Destroy()).ReturnsAsync(() => true).Callback(() => this.IsDestroyCalled = true);

            // カレントユーザーの登録
            container.RegisterInstance(typeof(User), UnityInstanceNames.CurrentUser, currentUser.Object);

            this.Actual = new UnsubscribePageViewModel().BuildUp(container);

            this.Actual.UnsubscribeCommand.Execute();

            while(!this.Actual.UnsubscribeCommand.CanExecute()) {}
        }

        [TestCase]
        public void IsShouldDestroyCalled()
        {
            Assert.That(this.IsDestroyCalled, Is.EqualTo(true));
        }
    }

    public class OnUnsubscribeTest : ViewModelTestBase
    {
        private bool IsDestroyCalled { get; set; }

        private UnsubscribePageViewModel Actual { get; set; }

        [SetUp]
        public void SetUp()
        {
            var container = this.GenerateUnityContainer();

            var currentUser = new Mock<User>();

            // カレントユーザーの登録
            container.RegisterInstance(typeof(User), UnityInstanceNames.CurrentUser, currentUser.Object);

            this.Actual = new UnsubscribePageViewModel(this.NavigationService).BuildUp(container);

            currentUser.Raise(x => x.DestroySuccess += null, EventArgs.Empty);
        }

        [TestCase]
        public void IsShouldNavigated()
        {
            Assert.That(this.NavigatePageName, Is.EqualTo("/SignInPage"));
        }
    }
}
