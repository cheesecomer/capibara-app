using System;
using System.Threading.Tasks;

using Capibara.Models;

using Moq;
using NUnit.Framework;

using SubjectViewModel = Capibara.ViewModels.UserViewModel;

namespace Capibara.Test.ViewModels.UserViewModelTest
{
    namespace OnResumeTest
    {
        public class WhenOther : ViewModelTestBase
        {
            public void ItShouldSendView()
            {
                this.IsolatedStorage.UserId = 2;

                var subject = new SubjectViewModel(model: new User { Id = 1 }).BuildUp(this.Container);

                subject.OnResume();

                this.Tracker.Verify(x => x.SendView(It.Is<string>(v => v == "/User/1")), Times.Once());
            }
        }

        public class WhenOwn : ViewModelTestBase
        {
            public void ItShouldSendView()
            {
                this.IsolatedStorage.UserId = 1;

                var subject = new SubjectViewModel(model: new User { Id = 1 }).BuildUp(this.Container);

                subject.OnResume();

                this.Tracker.Verify(x => x.SendView(It.IsAny<string>()), Times.Never());
            }
        }
    }

    public class NicknamePropertyTest : ViewModelTestBase
    {
        protected SubjectViewModel Subject;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var model = new User { Nickname = "xxxx", Id = Guid.NewGuid().ToInt() }.BuildUp(this.Container);

            this.Subject = new SubjectViewModel(model: model).BuildUp(this.Container);
        }

        [TestCase]
        public void ItShouldValueWithExpect()
        {
            Assert.That(this.Subject.Nickname.Value, Is.EqualTo("xxxx"));
        }

        [TestCase]
        public void ItShouldUpdate()
        {
            this.Subject.Model.Nickname = "xxxx!!!";
            Assert.That(this.Subject.Nickname.Value, Is.EqualTo("xxxx!!!"));
        }
    }

    public class BiographyPropertyTest : ViewModelTestBase
    {
        protected SubjectViewModel Subject;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var container = this.Container;
            var model = new User() { Biography = "xxxx", Id = Guid.NewGuid().ToInt() }.BuildUp(this.Container);
            this.Subject = new SubjectViewModel(model: model).BuildUp(this.Container);
        }

        [TestCase]
        public void ItShouldValueWithExpect()
        {
            Assert.That(this.Subject.Biography.Value, Is.EqualTo("xxxx"));
        }

        [TestCase]
        public void ItShouldUpdate()
        {
            this.Subject.Model.Biography = "xxxx!!!";
            Assert.That(this.Subject.Biography.Value, Is.EqualTo("xxxx!!!"));
        }
    }

    [TestFixture]
    public class RefreshCommandTest : ViewModelTestBase
    {
        private Mock<User> Model;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.Model = new Mock<User>();
            this.Model.SetupAllProperties();
            this.Model.Setup(x => x.Refresh()).ReturnsAsync(true);

            var viewModel = new SubjectViewModel(model: this.Model.Object).BuildUp(this.Container);

            viewModel.RefreshCommand.Execute();

            while (!viewModel.RefreshCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldShowDialog()
        {
            this.ProgressDialogService.Verify(x => x.DisplayProgressAsync(It.IsAny<Task>(), It.IsAny<string>()));
        }

        [TestCase]
        public void ItShouldRefreshCalled()
        {
            this.Model.Verify(x => x.Refresh(), Times.Once());
        }
    }
}
