using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Disposables;

using Capibara.Models;

using Prism.Services;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public ReactiveCollection<MenuItem> MenuItems { get; } =
            new ReactiveCollection<MenuItem>();

        public AsyncReactiveCommand<MenuItem> ItemTappedCommand { get; }

        public ReactiveProperty<string> Nickname { get; } = new ReactiveProperty<string>();

        public MainPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
            this.MenuItems.Add(new MenuItem { Name = "ホーム", PagePath = "NavigationPage/FloorMapPage" });
            this.MenuItems.Add(new MenuItem { Name = "プロフィール", PagePath = "NavigationPage/MyProfilePage" });
            this.MenuItems.Add(new MenuItem { Name = "設定", PagePath = "NavigationPage/SettingPage" });

            this.ItemTappedCommand = new AsyncReactiveCommand<MenuItem>();
            this.ItemTappedCommand.Subscribe(async x => await this.NavigationService.NavigateAsync(x.PagePath));
        }

        protected override void OnContainerChanged()
        {
            base.OnContainerChanged();

            this.CurrentUser.ObserveProperty(x => x.Nickname).Subscribe(x => this.Nickname.Value = x);
        }

        public class MenuItem
        {
            public string Name { get; set; }

            public string PagePath { get; set; }
        }
    }
}
