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
        public override void SetUp()
        {
            base.SetUp();

            var currentUser = new Mock<User>();

            currentUser.Setup(x => x.Destroy()).ReturnsAsync(() => true).Callback(() => this.IsDestroyCalled = true);

            // カレントユーザーの登録
            this.Container.RegisterInstance(typeof(User), UnityInstanceNames.CurrentUser, currentUser.Object);

            this.Subject = new SubjectViewModel().BuildUp(this.Container);

            this.Subject.UnsubscribeCommand.Execute();

            while(!this.Subject.UnsubscribeCommand.CanExecute()) {}
        }

        [TestCase]
        public void ItShouldDestroyCalled()
        {
            Assert.That(this.IsDestroyCalled, Is.EqualTo(true));
        }
    }

    public class OnUnsubscribeTest : ViewModelTestBase
    {
        private bool IsDestroyCalled { get; set; }

        private SubjectViewModel Subject { get; set; }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var currentUser = new Mock<User>();

            // カレントユーザーの登録
            this.Container.RegisterInstance(typeof(User), UnityInstanceNames.CurrentUser, currentUser.Object);

            this.Subject = new SubjectViewModel(this.NavigationService.Object).BuildUp(this.Container);

            currentUser.Raise(x => x.DestroySuccess += null, EventArgs.Empty);
        }

        [TestCase]
        public void ItShouldNavigated()
        {
            this.NavigationService.Verify(x => x.NavigateAsync("/SignUpPage", null), Times.Once());
        }
    }
}
