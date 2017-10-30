using System;

using Capibara.Models;
using Capibara.ViewModels;

using NUnit.Framework;
namespace Capibara.Test.ViewModels.ParticipantsPageViewModelTest
{
    namespace ParticipantsPropertyTest
    {
        [TestFixture]
        public class WhenUpdate : TestFixtureBase
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
