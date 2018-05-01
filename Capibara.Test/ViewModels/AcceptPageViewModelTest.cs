using System;
using System.Threading.Tasks;

using Capibara.ViewModels;
using Capibara.Models;
using Capibara.Services;

using Moq;
using NUnit.Framework;
using Unity;

using Prism.Services;

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
        public void ItShouldIsTermsUrl()
        {
            var subject = new SubjectViewModel().BuildUp(this.Container);
            Assert.That(subject.Source.Value.Url, Is.EqualTo(this.Environment.TermsUrl));
        }
    }

    public class ActiveCommandNamePropertyTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShouldIsNext()
        {
            var subject = new SubjectViewModel().BuildUp(this.Container);
            Assert.That(subject.ActiveCommandName.Value, Is.EqualTo("次へ"));
        }
    }

    public class TitlePropertyTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShouldIsTerms()
        {
            var subject = new SubjectViewModel().BuildUp(this.Container);
            Assert.That(subject.Title.Value, Is.EqualTo("利用規約の同意"));
        }
    }

    public class ActiveCommandPropertyTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShouldIsNext()
        {
            var subject = new SubjectViewModel().BuildUp(this.Container);
            Assert.That(subject.ActiveCommand.Value, Is.EqualTo(subject.NextCommand));
        }
    }

    public class NextCommandCanExecute : ViewModelTestBase
    {
        [TestCase("http://localhost:9999/terms", true, true)]
        [TestCase("http://localhost:9999/terms", false, false)]
        [TestCase("http://localhost:9999/privacy_policy", true, false)]
        [TestCase("http://localhost:9999/privacy_policy", false, false)]
        public void ItShouldIsPrivacyPolicyUrl(string url, bool isLoaded, bool expect)
        {
            var subject = new SubjectViewModel().BuildUp(this.Container);
            subject.Source.Value.Url = url;
            subject.IsLoaded.Value = isLoaded;
            Assert.That(subject.NextCommand.CanExecute(), Is.EqualTo(expect));
        }
    }

    public class NextCommandTest : ViewModelTestBase
    {
        private SubjectViewModel Subject;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var model = new Mock<User>();
            model.SetupAllProperties();

            this.Subject = new SubjectViewModel(this.NavigationService.Object, model: model.Object).BuildUp(this.Container);
            this.Subject.Source.Value.Url = "http://localhost:9999/terms";
            this.Subject.IsLoaded.Value = true;

            this.Subject.NextCommand.Execute();
        }

        [TestCase]
        public void ItShouldShowDPrivacyPolicy()
        {
            Assert.That(this.Subject.Source.Value.Url, Is.EqualTo("http://localhost:9999/privacy_policy"));
        }

        [TestCase]
        public void ItShouldIsNotLoaded()
        {
            Assert.That(this.Subject.IsLoaded.Value, Is.EqualTo(false));
        }

        [TestCase]
        public void ItShouldNextCommandCanNotExecute()
        {
            Assert.That(this.Subject.NextCommand.CanExecute(), Is.EqualTo(false));
        }

        [TestCase]
        public void ItShouldActiveCommandIsAgree()
        {
            Assert.That(this.Subject.ActiveCommand.Value, Is.EqualTo(this.Subject.AgreeCommand));
        }

        [TestCase]
        public void ItShouldActiveCommandNameIsAgree()
        {
            Assert.That(this.Subject.ActiveCommandName.Value, Is.EqualTo("同意する"));
        }

        [TestCase]
        public void ItShouldTitleIsPrivacyPolicy()
        {
            Assert.That(this.Subject.Title.Value, Is.EqualTo("プライバシーポリシーの同意"));
        }
    }

    public class AgreeCommandCanExecute : ViewModelTestBase
    {
        [TestCase("http://localhost:9999/terms", true, false)]
        [TestCase("http://localhost:9999/terms", false, false)]
        [TestCase("http://localhost:9999/privacy_policy", true, true)]
        [TestCase("http://localhost:9999/privacy_policy", false, false)]
        public void ItShouldIsPrivacyPolicyUrl(string url, bool isLoaded, bool expect)
        {
            var subject = new SubjectViewModel().BuildUp(this.Container);
            subject.Source.Value.Url = url;
            subject.IsLoaded.Value = isLoaded;
            Assert.That(subject.AgreeCommand.CanExecute(), Is.EqualTo(expect));
        }
    }

    public class AgreeCommandTest : ViewModelTestBase
    {
        private Mock<User> Model;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.Model = new Mock<User>();
            this.Model.SetupAllProperties();
            this.Model.Setup(x => x.Accept()).ReturnsAsync(true);

            var viewModel = new SubjectViewModel(this.NavigationService.Object, model: this.Model.Object).BuildUp(this.Container);
            viewModel.Source.Value.Url = "http://localhost:9999/privacy_policy";
            viewModel.IsLoaded.Value = true;

            viewModel.AgreeCommand.Execute();

            while (!viewModel.AgreeCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldShowDialog()
        {
            this.ProgressDialogService.Verify(x => x.DisplayProgressAsync(It.IsAny<Task>(), It.IsAny<string>()));
        }

        [TestCase]
        public void ItShouldCommitCalled()
        {
            this.Model.Verify(x => x.Accept(), Times.Once());
        }
    }

    public class CancelCommandTest : ViewModelTestBase
    {
        private Mock<User> Model;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.Model = new Mock<User>();
            this.Model.SetupAllProperties();
            this.Model.Setup(x => x.Destroy()).ReturnsAsync(true);

            var viewModel = new SubjectViewModel(this.NavigationService.Object, model: this.Model.Object).BuildUp(this.Container);

            viewModel.CancelCommand.Execute();

            while (!viewModel.CancelCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldShowDialog()
        {
            this.ProgressDialogService.Verify(x => x.DisplayProgressAsync(It.IsAny<Task>(), It.IsAny<string>()));
        }

        [TestCase]
        public void ItShouldDestroyCalled()
        {
            this.Model.Verify(x => x.Destroy(), Times.Once());
        }
    }

    public class OnAcceptSuccessTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShouldCommitCalled()
        {
            var model = new Mock<User>();

            new SubjectViewModel(this.NavigationService.Object, model: model.Object).BuildUp(this.Container);

            model.Raise(x => x.AcceptSuccess += null, EventArgs.Empty);

            this.NavigationService.Verify(x => x.NavigateAsync("/NavigationPage/MainPage"), Times.Once());
        }
    }

    public class OnDestroySuccessTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShouldCommitCalled()
        {
            var model = new Mock<User>();

            new SubjectViewModel(this.NavigationService.Object, model: model.Object).BuildUp(this.Container);

            model.Raise(x => x.DestroySuccess += null, EventArgs.Empty);

            this.NavigationService.Verify(x => x.NavigateAsync("/SignUpPage"), Times.Once());
        }
    }

    public class LoadedCommandTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShoulLoadedPropertyUpdate()
        {
            var viewModel = new SubjectViewModel().BuildUp(this.Container);
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
            var model = new Mock<User>();
            model.SetupAllProperties();

            var subject = new SubjectViewModel(model: model.Object);
            subject.BuildUp(this.Container);
            subject.OnNavigatedTo(null);

            model.VerifySet(x => x.IsAccepted = true);
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
