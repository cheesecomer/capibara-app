using System;
using System.Linq;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;

using Moq;
using NUnit.Framework;

using Xamarin.Forms;

using SubjectViewModel = Capibara.ViewModels.AcceptPageViewModel;

namespace Capibara.Test.ViewModels.AcceptPageViewModel
{
    public class SourcePropertyTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShouldIsPrivacyPolicyUrl()
        {
            var subject = new SubjectViewModel().BuildUp(this.GenerateUnityContainer());
            Assert.That((subject.Source.Value as UrlWebViewSource).Url, Is.EqualTo(this.Environment.PrivacyPolicyUrl));
        }
    }

    public class AgreeCommandCanExecute
    {
        [TestCase(true, true)]
        [TestCase(false, false)]
        public void ItShouldIsPrivacyPolicyUrl(bool isLoaded, bool expect)
        {
            var subject = new SubjectViewModel();
            subject.IsLoaded.Value = isLoaded;
            Assert.That(subject.AgreeCommand.CanExecute(), Is.EqualTo(expect));
        }
    }

    public class AgreeCommandTest : ViewModelTestBase
    {
        private bool IsCommitCalled;

        [SetUp]
        public void SetUp()
        {
            var model = new Mock<User>();
            model.SetupAllProperties();
            model.Setup(x => x.Commit()).ReturnsAsync(true).Callback(() => this.IsCommitCalled = true);

            var viewModel = new SubjectViewModel(this.NavigationService, model: model.Object).BuildUp(this.GenerateUnityContainer());
            viewModel.IsLoaded.Value = true;

            viewModel.AgreeCommand.Execute();

            while (!viewModel.AgreeCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldShowDialog()
        {
            Assert.That(this.IsDisplayedProgressDialog, Is.EqualTo(true));
        }

        [TestCase]
        public void ItShouldCommitCalled()
        {
            Assert.That(this.IsCommitCalled, Is.EqualTo(true));
        }
    }

    public class CancelCommandTest : ViewModelTestBase
    {
        private bool IsDestroyCalled;

        [SetUp]
        public void SetUp()
        {
            var model = new Mock<User>();
            model.SetupAllProperties();
            model.Setup(x => x.Destroy()).ReturnsAsync(true).Callback(() => this.IsDestroyCalled = true);

            var viewModel = new SubjectViewModel(this.NavigationService, model: model.Object).BuildUp(this.GenerateUnityContainer());

            viewModel.CancelCommand.Execute();

            while (!viewModel.CancelCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldShowDialog()
        {
            Assert.That(this.IsDisplayedProgressDialog, Is.EqualTo(true));
        }

        [TestCase]
        public void ItShouldDestroyCalled()
        {
            Assert.That(this.IsDestroyCalled, Is.EqualTo(true));
        }
    }

    public class OnCommitSuccessTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShouldCommitCalled()
        {
            var model = new Mock<User>();

            new SubjectViewModel(this.NavigationService, model: model.Object).BuildUp(this.GenerateUnityContainer());

            model.Raise(x => x.CommitSuccess += null, EventArgs.Empty);

            Assert.That(this.NavigatePageName, Is.EqualTo("/MainPage/NavigationPage/FloorMapPage"));
        }
    }

    public class OnDestroySuccessTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShouldCommitCalled()
        {
            var model = new Mock<User>();

            new SubjectViewModel(this.NavigationService, model: model.Object).BuildUp(this.GenerateUnityContainer());

            model.Raise(x => x.DestroySuccess += null, EventArgs.Empty);

            Assert.That(this.NavigatePageName, Is.EqualTo("/SignUpPage"));
        }
    }

    public class LoadedCommandTest
    {
        [TestCase]
        public void ItShoulLoadedPropertyUpdate()
        {
            var viewModel = new SubjectViewModel();
            viewModel.IsLoaded.Value = false;

            viewModel.LoadedCommand.Execute();

            Assert.That(viewModel.IsLoaded.Value, Is.EqualTo(true));
        }
    }
}
