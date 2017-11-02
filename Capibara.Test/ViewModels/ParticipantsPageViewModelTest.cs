using System.Net;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;

using Moq;
using NUnit.Framework;

using Prism.Navigation;
using Prism.Services;

namespace Capibara.Test.ViewModels.ParticipantsPageViewModelTest
{
    namespace ItemTappedCommand
    {
        [TestFixture]
        public class WhenOther : ViewModelTestBase
        {
            protected string NavigatePageName { get; private set; }

            protected NavigationParameters NavigationParameters { get; private set; }

            [SetUp]
            public void SetUp()
            {
                var navigationService = new Mock<INavigationService>();
                navigationService
                    .Setup(x => x.NavigateAsync(It.IsAny<string>(), It.IsAny<NavigationParameters>(), It.IsAny<bool?>(), It.IsAny<bool>()))
                    .Returns((string name, NavigationParameters parameters, bool? useModalNavigation, bool animated) =>
                    {
                        this.NavigatePageName = name;
                        this.NavigationParameters = parameters;
                        return Task.Run(() => { });
                    });

                var viewModel = new ParticipantsPageViewModel(navigationService.Object).BuildUp(this.GenerateUnityContainer());

                viewModel.ItemTappedCommand.Execute(new User { Id = 1 });

                while (!viewModel.ItemTappedCommand.CanExecute()) { }
            }

            [TestCase]
            public void ItShouldNavigateToParticipantsPage()
            {
                Assert.That(this.NavigatePageName, Is.EqualTo("UserProfilePage"));
            }

            [TestCase]
            public void ItShouldNavigationParametersHsaModel()
            {
                Assert.That(this.NavigationParameters.ContainsKey(ParameterNames.Model), Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldNavigationParameterModelIsRoom()
            {
                Assert.That(this.NavigationParameters[ParameterNames.Model] is User, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenOwn : ViewModelTestBase
        {
            protected string NavigatePageName { get; private set; }

            protected NavigationParameters NavigationParameters { get; private set; }

            [SetUp]
            public void SetUp()
            {
                var navigationService = new Mock<INavigationService>();
                navigationService
                    .Setup(x => x.NavigateAsync(It.IsAny<string>(), It.IsAny<NavigationParameters>(), It.IsAny<bool?>(), It.IsAny<bool>()))
                    .Returns((string name, NavigationParameters parameters, bool? useModalNavigation, bool animated) =>
                    {
                        this.NavigatePageName = name;
                        this.NavigationParameters = parameters;
                        return Task.Run(() => { });
                    });

                var viewModel = new ParticipantsPageViewModel(navigationService.Object).BuildUp(this.GenerateUnityContainer());

                this.IsolatedStorage.UserId = 1;

                viewModel.ItemTappedCommand.Execute(new User { Id = 1 });

                while (!viewModel.ItemTappedCommand.CanExecute()) { }
            }

            [TestCase]
            public void ItShouldNavigateToParticipantsPage()
            {
                Assert.That(this.NavigatePageName, Is.EqualTo("MyProfilePage"));
            }

            [TestCase]
            public void ItShouldNavigationParametersHsaModel()
            {
                Assert.That(this.NavigationParameters.ContainsKey(ParameterNames.Model), Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldNavigationParameterModelIsRoom()
            {
                Assert.That(this.NavigationParameters[ParameterNames.Model] is User, Is.EqualTo(true));
            }
        }
    }


    namespace ParticipantsPropertyTest
    {
        [TestFixture]
        public class WhenUpdate : ViewModelTestBase
        {
            protected ParticipantsPageViewModel ViewModel { get; private set; }

            [SetUp]
            public void SetUp()
            {
                this.ViewModel = new ParticipantsPageViewModel().BuildUp(this.GenerateUnityContainer());
            }

            [TestCase]
            public void ItShouldValueToExpect()
            {
                this.ViewModel.Model.Participants.Add(new User());
                Assert.That(this.ViewModel.Participants.Count, Is.EqualTo(1));
            }
        }
    }
}
