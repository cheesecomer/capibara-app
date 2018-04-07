
using Capibara.Models;
using Capibara.ViewModels;
using NUnit.Framework;
using SubjectViewModel = Capibara.ViewModels.MessageViewModel;

namespace Capibara.Test.ViewModels.MessageViewModel
{

    [TestFixture]
    public class ShowProfileCommandTest : ViewModelTestBase
    {
        private Message model;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.model = new Message { Sender = new User().BuildUp(this.Container) }.BuildUp(this.Container);

            var viewModel = new SubjectViewModel(this.NavigationService.Object, model: this.model);
            viewModel.BuildUp(this.Container);

            viewModel.ShowProfileCommand.Execute();

            while (!viewModel.ShowProfileCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldNavigateToUserProfilePage()
        {
            Assert.That(this.NavigatePageName, Is.EqualTo("UserProfilePage"));
        }

        [TestCase]
        public void ItShouldNavigationParametersHsaModel()
        {
            Assert.That(this.NavigationParameters.ContainsKey(ParameterNames.Model), Is.EqualTo(true));
        }

        [TestCase]
        public void ItShouldNavigationParameterModelIsExpect()
        {
            Assert.That(this.NavigationParameters[ParameterNames.Model], Is.EqualTo(this.model.Sender));
        }
    }
}
