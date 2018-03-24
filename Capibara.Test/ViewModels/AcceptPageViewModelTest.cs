using System;
using System.Linq;
using System.Threading.Tasks;

using Capibara.ViewModels;
using Capibara.Models;
using Capibara.Services;

using Moq;
using NUnit.Framework;
using Unity;

using Prism.Services;

using Xamarin.Forms;

using SubjectViewModel = Capibara.ViewModels.AcceptPageViewModel;

namespace Capibara.Test.ViewModels.AcceptPageViewModel
{
    public class ViewModelTestBase : ViewModels.ViewModelTestBase
    {
        public bool IsOverrideUrlCalled;

        public override void SetUp()
        {
            base.SetUp();

            var overrideUrlService = new Mock<IOverrideUrlService>();
            overrideUrlService
                .Setup(x => x.OverrideUrl(It.IsAny<IDeviceService>(), It.IsAny<string[]>()))
                .Returns<IDeviceService, string[]>((x, y) => (IOverrideUrlCommandParameters v) => this.IsOverrideUrlCalled = true);

            this.Container.RegisterInstance(overrideUrlService.Object);
        }
    }

    public class SourcePropertyTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShouldIsPrivacyPolicyUrl()
        {
            var subject = new SubjectViewModel().BuildUp(this.Container);
            Assert.That(subject.Source.Value.Url, Is.EqualTo(this.Environment.PrivacyPolicyUrl));
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
        public override void SetUp()
        {
            base.SetUp();

            var model = new Mock<User>();
            model.SetupAllProperties();
            model.Setup(x => x.Commit()).ReturnsAsync(true).Callback(() => this.IsCommitCalled = true);

            var viewModel = new SubjectViewModel(this.NavigationService, model: model.Object).BuildUp(this.Container);
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
        public override void SetUp()
        {
            base.SetUp();

            var model = new Mock<User>();
            model.SetupAllProperties();
            model.Setup(x => x.Destroy()).ReturnsAsync(true).Callback(() => this.IsDestroyCalled = true);

            var viewModel = new SubjectViewModel(this.NavigationService, model: model.Object).BuildUp(this.Container);

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

            new SubjectViewModel(this.NavigationService, model: model.Object).BuildUp(this.Container);

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

            new SubjectViewModel(this.NavigationService, model: model.Object).BuildUp(this.Container);

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

    public class OnNavigatedToTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShouldAccepted()
        {
            bool isSetAcceptedTrue = false;

            var model = new Mock<User>();
            model.SetupAllProperties();
            model.SetupSet(x => x.IsAccepted = true).Callback(() => isSetAcceptedTrue = true);

            var subject = new SubjectViewModel(model: model.Object);
            subject.BuildUp(this.Container);
            subject.OnNavigatedTo(null);
            Assert.That(isSetAcceptedTrue, Is.EqualTo(true));
        }
    }

    public class OverrideUrlCommandTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShoulLoadedPropertyUpdate()
        {
            var viewModel = new SubjectViewModel().BuildUp(this.Container);
            viewModel.OverrideUrlCommand.Execute(new Mock<IOverrideUrlCommandParameters>().Object);

            Assert.That(this.IsOverrideUrlCalled, Is.EqualTo(true));
        }
    }
}
