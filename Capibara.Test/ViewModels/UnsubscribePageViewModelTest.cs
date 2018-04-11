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
        private Mock<User> Model { get; set; }

        private SubjectViewModel Subject { get; set; }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.Model = new Mock<User>();

            this.Model.Setup(x => x.Destroy()).ReturnsAsync(() => true);

            // カレントユーザーの登録
            this.Container.RegisterInstance(typeof(User), UnityInstanceNames.CurrentUser, this.Model.Object);

            this.Subject = new SubjectViewModel().BuildUp(this.Container);

            this.Subject.UnsubscribeCommand.Execute();

            while(!this.Subject.UnsubscribeCommand.CanExecute()) {}
        }

        [TestCase]
        public void ItShouldDestroyCalled()
        {
            this.Model.Verify(x => x.Destroy(), Times.Once());
        }
    }

    public class OnUnsubscribeTest : ViewModelTestBase
    {
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
