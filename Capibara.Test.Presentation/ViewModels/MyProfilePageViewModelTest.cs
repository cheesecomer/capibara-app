#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using Capibara.Presentation.Navigation;
using Moq;
using NUnit.Framework;
using Prism.Navigation;

namespace Capibara.Presentation.ViewModels
{
    public class MyProfilePageViewModelTest
    {
        #region EditCommand

        [Test]
        public void EditCommand_ShoulldNavigateToEditProfilePage()
        {
            var navigationService = Mock.NavigationService();
            var viewModel = new MyProfilePageViewModel(navigationService.Object);

            viewModel.EditCommand.Execute();

            navigationService
                .Verify(
                    x =>
                        x.NavigateAsync(
                            "EditProfilePage",
                            It.Is<NavigationParameters>(parameters =>
                                    parameters.Count == 1
                                &&  parameters[ParameterNames.Model] == viewModel.Model),
                            null,
                            true),
                    Times.Once());
        }

        #endregion

        #region SettingCommand

        [Test]
        public void SettingCommand_ShouldNavigateToSettingPage()
        {
            var navigationService = Mock.NavigationService();
            var viewModel = new MyProfilePageViewModel(navigationService.Object);

            viewModel.SettingCommand.Execute();

            navigationService
                .Verify(
                    x =>
                        x.NavigateAsync(
                            "SettingPage",
                            null,
                            null,
                            true),
                    Times.Once());
        }

        #endregion

        #region ShowFriendsCommand

        [Test]
        public void ShowFriendsCommand_ShoulldNavigateToFollowUsersPage()
        {
            var navigationService = Mock.NavigationService();
            var viewModel = new MyProfilePageViewModel(navigationService.Object);

            viewModel.ShowFriendsCommand.Execute();

            navigationService
                .Verify(
                    x =>
                        x.NavigateAsync(
                            "FollowUsersPage",
                            null,
                            null,
                            true),
                    Times.Once());
        }

        #endregion
    }
}
