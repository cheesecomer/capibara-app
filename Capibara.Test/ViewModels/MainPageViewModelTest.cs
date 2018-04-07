﻿using System.Linq;
using Capibara.Models;
using NUnit.Framework;
using Unity;
using MenuItem = Capibara.ViewModels.MainPageViewModel.MenuItem;
using SubjectViewModel = Capibara.ViewModels.MainPageViewModel;

namespace Capibara.Test.ViewModels.MainPageViewModel
{
    [TestFixture("FloorMapPage")]
    [TestFixture("MyProfilePage")]
    [TestFixture("SettingPage")]
    public class ItemTappedCommandTest : ViewModelTestBase
    {
        private string pagePath;

        public ItemTappedCommandTest(string pagePath)
        {
            this.pagePath = pagePath;
        }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var viewModel = new SubjectViewModel(this.NavigationService.Object);

            viewModel.ItemTappedCommand.Execute(new MenuItem { PagePath = this.pagePath} );

            while (!viewModel.ItemTappedCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldNavigateToParticipantsPage()
        {
            Assert.That(this.NavigatePageName, Is.EqualTo(this.pagePath));
        }
    }

    public class NicknamePropertyTest : ViewModelTestBase
    {
        protected SubjectViewModel Subject;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.Container.RegisterInstance(typeof(User), UnityInstanceNames.CurrentUser, new User() { Nickname = "xxxx"});

            this.Subject = new SubjectViewModel().BuildUp(this.Container);
        }

        [TestCase]
        public void ItShouldValueWithExpect()
        {
            Assert.That(this.Subject.Nickname.Value, Is.EqualTo("xxxx"));
        }

        [TestCase]
        public void ItShouldUpdate()
        {
            this.Subject.CurrentUser.Nickname = "xxxx!!!";
            Assert.That(this.Subject.Nickname.Value, Is.EqualTo("xxxx!!!"));
        }
    }

    public class MenuItemsPropertyTest : ViewModelTestBase
    {
        protected SubjectViewModel Subject;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.Subject = new SubjectViewModel();
        }

        [TestCase]
        public void ItShouldCountExpected()
        {
            Assert.That(this.Subject.MenuItems.Count, Is.EqualTo(4));
        }
    }

    [TestFixture(0, "ホーム", "NavigationPage/FloorMapPage")]
    [TestFixture(1, "プロフィール", "NavigationPage/MyProfilePage")]
    [TestFixture(2, "お知らせ", "NavigationPage/InformationsPage")]
    [TestFixture(3, "設定", "NavigationPage/SettingPage")]
    public class MenuItemsItemPropertyTest : ViewModelTestBase
    {
        protected SubjectViewModel Subject;

        private int index;

        private string name;

        private string pagePath;

        public MenuItemsItemPropertyTest(int index, string name, string pagePath)
        {
            this.index = index;
            this.name = name;
            this.pagePath = pagePath;
        }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.Subject = new SubjectViewModel();
        }

        [TestCase]
        public void ItShouldFirstItemNameWithExpect()
        {
            Assert.That(this.Subject.MenuItems.ElementAtOrDefault(index)?.Name, Is.EqualTo(name));
        }

        [TestCase]
        public void ItShouldFirstItemPagePathWithExpect()
        {
            Assert.That(this.Subject.MenuItems.ElementAtOrDefault(index)?.PagePath, Is.EqualTo(pagePath));
        }
    }
}
