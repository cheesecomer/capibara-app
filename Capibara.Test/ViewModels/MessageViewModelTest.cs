
using Capibara.Models;
using Capibara.ViewModels;
using Moq;
using Prism.Navigation;
using NUnit.Framework;
using SubjectViewModel = Capibara.ViewModels.MessageViewModel;

namespace Capibara.Test.ViewModels.MessageViewModel
{
    [TestFixture]
    public class ShowProfileCommandTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShouldNavigateToUserProfilePage()
        {
            var model = new Message { Sender = new User().BuildUp(this.Container), Content = string.Empty }.BuildUp(this.Container);

            var viewModel = new SubjectViewModel(this.NavigationService.Object, model: model);
            viewModel.BuildUp(this.Container);

            viewModel.ShowProfileCommand.Execute();

            while (!viewModel.ShowProfileCommand.CanExecute()) { }
            this.NavigationService.Verify(
                x => x.NavigateAsync(
                    "UserProfilePage", 
                    It.Is<NavigationParameters>(v => v.GetValueOrDefault(ParameterNames.Model) == model.Sender))
                , Times.Once());
        }
    }
}
