using System;

using Moq;
using NUnit.Framework;
using Unity;

using SubjectViewModel = Capibara.ViewModels.AboutPageViewModel;

namespace Capibara.Test.ViewModels
{
    public class AboutPageViewModel : ViewModelTestBase
    {
        [TestCase]
        public void VersionPropertyTest()
        {
            this.ApplicationService.SetupGet(x => x.AppVersion).Returns("1.9");
            var subject = new SubjectViewModel(this.NavigationService).BuildUp(this.Container);

            Assert.That(subject.Version.Value, Is.EqualTo("Version 1.9"));
        }

        [TestCase]
        public void CloseCommandTest()
        {
            var subject = new SubjectViewModel(this.NavigationService).BuildUp(this.Container);

            subject.CloseCommand.Execute();

            Assert.That(this.IsGoBackCalled, Is.EqualTo(true));
        }
    }
}
