using System;
using System.Linq;

using Capibara.Models;

using Moq;
using NUnit.Framework;
using SubjectViewModel = Capibara.ViewModels.ReportPageViewModel;

namespace Capibara.Test.ViewModels.ReportPageViewModel
{
    public class ReportReasonsPropertyTest : ViewModelTestBase
    {
        protected User Model;

        protected SubjectViewModel Subject;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var container = this.Container;

            this.Subject = new SubjectViewModel().BuildUp(container);
        }

        [TestCase]
        public void ItShould6()
        {
            Assert.That(this.Subject.ReportReasons.Count, Is.EqualTo(6));
        }

        [TestCase(0, ReportReason.Spam)]
        [TestCase(1, ReportReason.AbusiveOrHatefulSpeech)]
        [TestCase(2, ReportReason.AbusiveOrHatefulImage)]
        [TestCase(3, ReportReason.ObsceneSpeech)]
        [TestCase(4, ReportReason.ObsceneImage)]
        [TestCase(5, ReportReason.Other)]
        public void ItShouldItem(int index, ReportReason expect)
        {
            Assert.That(this.Subject.ReportReasons.ElementAtOrDefault(index), Is.EqualTo(expect));
        }
    }

    public class SelectedItemTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShouldDefaultSpam()
        {
            var viewModel = new SubjectViewModel();
            Assert.That(viewModel.SelectedItem.Value, Is.EqualTo(ReportReason.Spam));
        }

        [TestCase]
        public void ItShouldUpdate()
        {
            var viewModel = new SubjectViewModel();
            viewModel.SelectedIndex.Value = 1;
            Assert.That(viewModel.SelectedItem.Value, Is.EqualTo(ReportReason.AbusiveOrHatefulSpeech));
        }
    }

    public class SelectedIndexTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShouldDefaultSpam()
        {
            var viewModel = new SubjectViewModel();
            Assert.That(viewModel.SelectedIndex.Value, Is.EqualTo(0));
        }
    }

    public class SubmitCommandCanExecuteTest
    {
        [TestCase(ReportReason.Spam, "", true)]
        [TestCase(ReportReason.AbusiveOrHatefulSpeech, "", true)]
        [TestCase(ReportReason.AbusiveOrHatefulImage, "", true)]
        [TestCase(ReportReason.ObsceneSpeech, "", true)]
        [TestCase(ReportReason.ObsceneImage, "", true)]
        [TestCase(ReportReason.Other, "", false)]
        [TestCase(ReportReason.Spam, "foo bar", true)]
        [TestCase(ReportReason.AbusiveOrHatefulSpeech, "foo bar", true)]
        [TestCase(ReportReason.AbusiveOrHatefulImage, "foo bar", true)]
        [TestCase(ReportReason.ObsceneSpeech, "foo bar", true)]
        [TestCase(ReportReason.ObsceneImage, "foo bar", true)]
        [TestCase(ReportReason.Other, "foo bar", true)]
        [TestCase(ReportReason.Spam, " ", true)]
        [TestCase(ReportReason.AbusiveOrHatefulSpeech, " ", true)]
        [TestCase(ReportReason.AbusiveOrHatefulImage, " ", true)]
        [TestCase(ReportReason.ObsceneSpeech, " ", true)]
        [TestCase(ReportReason.ObsceneImage, " ", true)]
        [TestCase(ReportReason.Other, " ", false)]
        [TestCase(ReportReason.Spam, "　", true)]
        [TestCase(ReportReason.AbusiveOrHatefulSpeech, "　", true)]
        [TestCase(ReportReason.AbusiveOrHatefulImage, "　", true)]
        [TestCase(ReportReason.ObsceneSpeech, "　", true)]
        [TestCase(ReportReason.ObsceneImage, "　", true)]
        [TestCase(ReportReason.Other, "　", false)]
        public void ItShouldExpected(ReportReason reportReason, string message, bool expect)
        {
            var viewModel = new SubjectViewModel();
            viewModel.SelectedItem.Value = reportReason;
            viewModel.Message.Value = message;
            Assert.That(viewModel.SubmitCommand.CanExecute(), Is.EqualTo(expect));
        }
    }

    public class SubmitCommandTest : ViewModelTestBase
    {
        private bool IsReportCalled;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var model = new Mock<User>();
            model.SetupAllProperties();
            model.Setup(x => x.Report(It.IsAny<ReportReason>(), It.IsAny<string>())).ReturnsAsync(true).Callback(() => this.IsReportCalled = true);

            var viewModel = new SubjectViewModel(this.NavigationService.Object, model: model.Object).BuildUp(this.Container);

            viewModel.SubmitCommand.Execute();

            while (!viewModel.SubmitCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldShowDialog()
        {
            Assert.That(this.IsDisplayedProgressDialog, Is.EqualTo(true));
        }

        [TestCase]
        public void ItShouldCommitCalled()
        {
            Assert.That(this.IsReportCalled, Is.EqualTo(true));
        }
    }

    public class OnReportSuccessTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShouldCommitCalled()
        {
            var model = new Mock<User>();

            new SubjectViewModel(this.NavigationService.Object, model: model.Object).BuildUp(this.Container);

            model.Raise(x => x.ReportSuccess += null, EventArgs.Empty);

            Assert.That(this.IsGoBackCalled, Is.EqualTo(true));
        }
    }
}
