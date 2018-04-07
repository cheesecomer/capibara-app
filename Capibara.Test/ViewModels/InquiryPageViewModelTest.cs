using System;
using System.Threading.Tasks;

using Capibara.Net;

using Moq;
using NUnit.Framework;
using SubjectViewModel = Capibara.ViewModels.InquiryPageViewModel;

namespace Capibara.Test.ViewModels.InquiryPageViewModelTest
{
    public class SubmitCommandCanExecuteTest
    {
        [TestCase("", "", false)]
        [TestCase(" ", "", false)]
        [TestCase("", " ", false)]
        [TestCase(" ", " ", false)]
        [TestCase("　", "", false)]
        [TestCase("", "　", false)]
        [TestCase("　", "　", false)]
        [TestCase("example@email.com", "", false)]
        [TestCase("example@email.com", " ", false)]
        [TestCase("example@email.com", "　", false)]
        [TestCase("", "Message!", false)]
        [TestCase(" ", "Message!", false)]
        [TestCase("　", "Message!", false)]
        [TestCase("example@email.com", "Message!", true)]
        public void ItShouldExpected(string email, string message, bool expect)
        {
            var viewModel = new SubjectViewModel();
            viewModel.Email.Value = email;
            viewModel.Message.Value = message;

            Assert.That(viewModel.SubmitCommand.CanExecute(), Is.EqualTo(expect));
        }
    }

    public class SubmitCommandTest : ViewModelTestBase
    {
        private Mock<RequestBase> Request;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.Request = new Mock<RequestBase>();
            this.Request.Setup(x => x.Execute()).Returns(Task.CompletedTask);

            var container = this.Container;
            this.RequestFactory.Setup(x => x.InquiriesCreateRequest("example@email.com", "Message!"))
                .Returns(this.Request.Object);
            
            var viewModel = new SubjectViewModel(this.NavigationService.Object).BuildUp(container);
            viewModel.Email.Value = "example@email.com";
            viewModel.Message.Value = "Message!";

            viewModel.SubmitCommand.Execute();

            while (!viewModel.SubmitCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldShowDialog()
        {
            this.ProgressDialogService.Verify(x => x.DisplayProgressAsync(It.IsAny<Task>(), It.IsAny<string>()));
        }

        [TestCase]
        public void ItShouldGoBackCalled()
        {
            this.NavigationService.Verify(x => x.GoBackAsync(), Times.Once());
        }

        [TestCase]
        public void ItShouldInquiriesCreateRequestCalled()
        {
            this.Request.Verify(x => x.Execute(), Times.Once());
        }
    }
}
