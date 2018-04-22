using Capibara.ViewModels;

using Moq;
using NUnit.Framework;
using Prism.Navigation;

using SubjectViewModel = Capibara.ViewModels.MyProfilePageViewModel;

namespace Capibara.Test.ViewModels.MyProfilePageViewModel
{
    [TestFixture]
    public class EditCommandTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShouldNavigateToEditProfilePage()
        {
            var viewModel = new SubjectViewModel(this.NavigationService.Object);

            viewModel.EditCommand.Execute();

            while (!viewModel.EditCommand.CanExecute()) { }

            this.NavigationService.Verify(
                x => x.NavigateAsync(
                    "EditProfilePage",
                    It.Is<NavigationParameters>(v => v.GetValueOrDefault(ParameterNames.Model) == viewModel.Model)),
                Times.Once());
        }
    }
}
