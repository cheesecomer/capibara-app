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

namespace Capibara.Test.ViewModels.MessageViewModelTest
{

    [TestFixture]
    public class ShowProfileCommandTest : ViewModelTestBase
    {
        private Message model;

        protected string NavigatePageName { get; private set; }

        protected NavigationParameters NavigationParameters { get; private set; }

        [SetUp]
        public void SetUp()
        {
            var container = this.GenerateUnityContainer();

            this.model = new Message { Sender = new User().BuildUp(container) }.BuildUp(container);

            var navigationService = new Mock<INavigationService>();
            navigationService
                .Setup(x => x.NavigateAsync(It.IsAny<string>(), It.IsAny<NavigationParameters>(), It.IsAny<bool?>(), It.IsAny<bool>()))
                .Returns((string name, NavigationParameters parameters, bool? useModalNavigation, bool animated) =>
                {
                    this.NavigatePageName = name;
                    this.NavigationParameters = parameters;
                    return Task.Run(() => { });
                });

            var viewModel = new MessageViewModel(navigationService.Object, model: this.model);
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
