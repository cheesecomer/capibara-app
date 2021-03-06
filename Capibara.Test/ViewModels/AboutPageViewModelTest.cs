﻿using System;

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
            var subject = new SubjectViewModel(this.NavigationService.Object).BuildUp(this.Container);

            Assert.That(subject.Version.Value, Is.EqualTo("Version 1.9"));
        }

        [TestCase]
        public void CloseCommandTest()
        {
            var subject = new SubjectViewModel(this.NavigationService.Object).BuildUp(this.Container);

            subject.CloseCommand.Execute();

            this.NavigationService.Verify(x => x.GoBackAsync(), Times.Once());
        }
    }
}
