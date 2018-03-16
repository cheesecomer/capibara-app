using System;
using System.Reactive.Linq;

using Prism.Navigation;
using Prism.Services;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Xamarin.Forms;

namespace Capibara.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public ReactiveCollection<MenuItem> MenuItems { get; } =
            new ReactiveCollection<MenuItem>();

        public AsyncReactiveCommand<MenuItem> ItemTappedCommand { get; }

        public ReactiveProperty<ImageSource> Icon { get; }

        public ReactiveProperty<string> Nickname { get; } = new ReactiveProperty<string>();

        public MainPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
            this.MenuItems.Add(new MenuItem { Name = "ホーム", PagePath = "NavigationPage/FloorMapPage" });
            this.MenuItems.Add(new MenuItem { Name = "プロフィール", PagePath = "NavigationPage/MyProfilePage" });
            this.MenuItems.Add(new MenuItem { Name = "お知らせ", PagePath = "NavigationPage/InformationsPage" });
            this.MenuItems.Add(new MenuItem { Name = "設定", PagePath = "NavigationPage/SettingPage" });

            this.Icon = new ReactiveProperty<ImageSource>()
                .AddTo(this.Disposable);

            this.ItemTappedCommand = new AsyncReactiveCommand<MenuItem>();
            this.ItemTappedCommand.Subscribe(async x => await this.NavigationService.NavigateAsync(x.PagePath, x.Parameters));
        }

        protected override void OnContainerChanged()
        {
            base.OnContainerChanged();

            this.CurrentUser.ObserveProperty(x => x.Nickname).Subscribe(x => this.Nickname.Value = x);

            this.MenuItems[1].Parameters = new NavigationParameters { { ParameterNames.Model, this.CurrentUser } };
            this.CurrentUser
                .ObserveProperty(x => x.IconUrl)
                .Subscribe(x => this.Icon.Value = this.CurrentUser.IconUrl.IsNullOrEmpty() ? null : ImageSource.FromUri(new Uri(x)));
        }

        public class MenuItem
        {
            public string Name { get; set; }

            public string PagePath { get; set; }

            public NavigationParameters Parameters { get; set; }
        }
    }
}
