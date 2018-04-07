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
        private bool IsInquiriesCreateRequestCalled;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var request = new Mock<RequestBase>();
            request.Setup(x => x.Execute()).Returns(Task.CompletedTask);

            var container = this.Container;
            this.RequestFactory.Setup(x => x.InquiriesCreateRequest("example@email.com", "Message!"))
                .Returns(request.Object)
                .Callback<string, string>((a, b) => this.IsInquiriesCreateRequestCalled = true);

            var viewModel = new SubjectViewModel(this.NavigationService.Object).BuildUp(container);
            viewModel.Email.Value = "example@email.com";
            viewModel.Message.Value = "Message!";

            viewModel.SubmitCommand.Execute();

            while (!viewModel.SubmitCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldShowDialog()
        {
            Assert.That(this.IsDisplayedProgressDialog, Is.EqualTo(true));
        }

        [TestCase]
        public void ItShouldGoBackCalled()
        {
            Assert.That(this.IsGoBackCalled, Is.EqualTo(true));
        }

        [TestCase]
        public void ItShouldInquiriesCreateRequestCalled()
        {
            Assert.That(this.IsInquiriesCreateRequestCalled, Is.EqualTo(true));
        }
    }
}
