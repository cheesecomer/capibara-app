using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;

using Microsoft.Practices.Unity;

using Moq;
using NUnit.Framework;

using Prism.Navigation;
using Prism.Services;

namespace Capibara.Test.ViewModels.ReportPageViewModelTest
{
    public class ReportReasonsPropertyTest : ViewModelTestBase
    {
        protected User Model;

        protected ReportPageViewModel Subject;

        [SetUp]
        public void SetUp()
        {
            var container = this.GenerateUnityContainer();

            this.Subject = new ReportPageViewModel().BuildUp(container);
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
            var viewModel = new ReportPageViewModel();
            Assert.That(viewModel.SelectedItem.Value, Is.EqualTo(ReportReason.Spam));
        }
    }

    public class ReportCommandCanExecuteTest
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
            var viewModel = new ReportPageViewModel();
            viewModel.SelectedItem.Value = reportReason;
            viewModel.Message.Value = message;
            Assert.That(viewModel.ReportCommand.CanExecute(), Is.EqualTo(expect));
        }
    }

    public class ReportCommandTest : ViewModelTestBase
    {
        private bool IsReportCalled;

        [SetUp]
        public void SetUp()
        {
            var model = new Mock<User>();
            model.SetupAllProperties();
            model.Setup(x => x.Report(It.IsAny<ReportReason>(), It.IsAny<string>())).ReturnsAsync(true).Callback(() => this.IsReportCalled = true);

            var viewModel = new ReportPageViewModel(this.NavigationService, model: model.Object).BuildUp(this.GenerateUnityContainer());

            viewModel.ReportCommand.Execute();

            while (!viewModel.ReportCommand.CanExecute()) { };
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

            new ReportPageViewModel(this.NavigationService, model: model.Object).BuildUp(this.GenerateUnityContainer());

            model.Raise(x => x.ReportSuccess += null, EventArgs.Empty);

            Assert.That(this.IsGoBackCalled, Is.EqualTo(true));
        }
    }
}
