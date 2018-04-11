using Capibara.Models;
using Capibara.ViewModels;
using NUnit.Framework;
using Moq;
using Prism.Navigation;
using SubjectViewModel = Capibara.ViewModels.ParticipantsPageViewModel;

namespace Capibara.Test.ViewModels.ParticipantsPageViewModel
{
    namespace ItemTappedCommand
    {
        [TestFixture]
        public class WhenOther : ViewModelTestBase
        {
            [TestCase]
            public void ItShouldNavigateToUserProfilePage()
            {
                var viewModel = new SubjectViewModel(this.NavigationService.Object).BuildUp(this.Container);
                var model = new User { Id = 1 };
                viewModel.ItemTappedCommand.Execute(new UserViewModel(model: model));

                while (!viewModel.ItemTappedCommand.CanExecute()) { }

                this.NavigationService.Verify(
                    x => x.NavigateAsync(
                        "UserProfilePage",
                        It.Is<NavigationParameters>(v => v.GetValueOrDefault(ParameterNames.Model) == model))
                    , Times.Once());
            }
        }

        [TestFixture]
        public class WhenOwn : ViewModelTestBase
        {
            [TestCase]
            public void ItShouldNavigateToMyProfilePage()
            {
                base.SetUp();

                var viewModel = new SubjectViewModel(this.NavigationService.Object).BuildUp(this.Container);

                this.IsolatedStorage.UserId = 1;

                var model = new User { Id = 1 };
                viewModel.ItemTappedCommand.Execute(new UserViewModel(model: model));

                while (!viewModel.ItemTappedCommand.CanExecute()) { }

                this.NavigationService.Verify(
                    x => x.NavigateAsync(
                        "MyProfilePage",
                        It.Is<NavigationParameters>(v => v.GetValueOrDefault(ParameterNames.Model) == viewModel.CurrentUser))
                    , Times.Once());
            }
        }
    }


    namespace ParticipantsPropertyTest
    {
        [TestFixture]
        public class WhenUpdate : ViewModelTestBase
        {
            protected SubjectViewModel Subject { get; private set; }

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Subject = new SubjectViewModel().BuildUp(this.Container);
            }

            [TestCase]
            public void ItShouldValueToExpect()
            {
                this.Subject.Model.Participants.Add(new User());
                Assert.That(this.Subject.Participants.Count, Is.EqualTo(1));
            }
        }
    }
}
