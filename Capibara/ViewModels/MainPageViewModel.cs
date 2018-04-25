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
            new ReactiveCollection<MenuItem>
        {
            new MenuItem { Name = "ホーム", PagePath = "NavigationPage/FloorMapPage" },
            new MenuItem { Name = "ダイレクトメッセージ", PagePath = "NavigationPage/InboxPage" },
            new MenuItem { Name = "プロフィール", PagePath = "NavigationPage/MyProfilePage" },
            new MenuItem { Name = "お知らせ", PagePath = "NavigationPage/InformationsPage" },
            new MenuItem { Name = "設定", PagePath = "NavigationPage/SettingPage" }
        };

        public AsyncReactiveCommand<MenuItem> ItemTappedCommand { get; }

        public ReactiveProperty<ImageSource> Icon { get; }

        public ReactiveProperty<string> Nickname { get; } = new ReactiveProperty<string>();

        public MainPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
            this.Icon = new ReactiveProperty<ImageSource>()
                .AddTo(this.Disposable);

            this.ItemTappedCommand = new AsyncReactiveCommand<MenuItem>();
            this.ItemTappedCommand.Subscribe(async x => await this.NavigationService.NavigateAsync(x.PagePath, x.Parameters));
        }

        protected override void OnContainerChanged()
        {
            base.OnContainerChanged();

            this.CurrentUser.ObserveProperty(x => x.Nickname).Subscribe(x => this.Nickname.Value = x);

            this.MenuItems[2].Parameters = new NavigationParameters { { ParameterNames.Model, this.CurrentUser } };
            this.CurrentUser
                .ObserveProperty(x => x.IconThumbnailUrl)
                .Subscribe(x => this.Icon.Value = x.IsNullOrEmpty() ? null : this.ImageSourceFactory.FromUri(new Uri(x)));
        }

        public class MenuItem
        {
            public string Name { get; set; }

            public string PagePath { get; set; }

            public NavigationParameters Parameters { get; set; }
        }
    }
}
