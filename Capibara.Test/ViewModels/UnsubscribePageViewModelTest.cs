using System;

using Capibara.Models;
using Capibara.ViewModels;

using Moq;
using NUnit.Framework;

using Unity;

using SubjectViewModel = Capibara.ViewModels.UnsubscribePageViewModel;

namespace Capibara.Test.ViewModels.UnsubscribePageViewModel
{
    public class UnsubscribeCommandTest : ViewModelTestBase
    {
        private bool IsDestroyCalled { get; set; }

        private SubjectViewModel Subject { get; set; }

        [SetUp]
        public void SetUp()
        {
            var container = this.GenerateUnityContainer();

            var currentUser = new Mock<User>();

            currentUser.Setup(x => x.Destroy()).ReturnsAsync(() => true).Callback(() => this.IsDestroyCalled = true);

            // カレントユーザーの登録
            container.RegisterInstance(typeof(User), UnityInstanceNames.CurrentUser, currentUser.Object);

            this.Subject = new SubjectViewModel().BuildUp(container);

            this.Subject.UnsubscribeCommand.Execute();

            while(!this.Subject.UnsubscribeCommand.CanExecute()) {}
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

        private SubjectViewModel Subject { get; set; }

        [SetUp]
        public void SetUp()
        {
            var container = this.GenerateUnityContainer();

            var currentUser = new Mock<User>();

            // カレントユーザーの登録
            container.RegisterInstance(typeof(User), UnityInstanceNames.CurrentUser, currentUser.Object);

            this.Subject = new SubjectViewModel(this.NavigationService).BuildUp(container);

            currentUser.Raise(x => x.DestroySuccess += null, EventArgs.Empty);
        }

        [TestCase]
        public void IsShouldNavigated()
        {
            Assert.That(this.NavigatePageName, Is.EqualTo("/SignUpPage"));
        }
    }
}
