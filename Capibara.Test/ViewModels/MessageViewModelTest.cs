﻿
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
        public void SetUp()
        {
            var container = this.GenerateUnityContainer();

            this.model = new Message { Sender = new User().BuildUp(container) }.BuildUp(container);

            var viewModel = new SubjectViewModel(this.NavigationService, model: this.model);
            viewModel.BuildUp(container);

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
