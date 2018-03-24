﻿using Capibara.Models;
using Capibara.ViewModels;
using NUnit.Framework;
using SubjectViewModel = Capibara.ViewModels.ParticipantsPageViewModel;

namespace Capibara.Test.ViewModels.ParticipantsPageViewModel
{
    namespace ItemTappedCommand
    {
        [TestFixture]
        public class WhenOther : ViewModelTestBase
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                var viewModel = new SubjectViewModel(this.NavigationService).BuildUp(this.Container);

                viewModel.ItemTappedCommand.Execute(new UserViewModel(model: new User { Id = 1 }));

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
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                var viewModel = new SubjectViewModel(this.NavigationService).BuildUp(this.Container);

                this.IsolatedStorage.UserId = 1;

                viewModel.ItemTappedCommand.Execute(new UserViewModel(model: new User { Id = 1 }));

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
