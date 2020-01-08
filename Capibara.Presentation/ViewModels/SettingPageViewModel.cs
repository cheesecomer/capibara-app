using System;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;

using Capibara.Domain.UseCases;

namespace Capibara.Presentation.ViewModels
{
    public class SettingPageViewModel: ViewModelBase
    {
        public SettingPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
            this.ItemTappedCommand = new AsyncReactiveCommand<SettingItem>();
            this.ItemTappedCommand.Subscribe(x => this.NavigationService.NavigateAsync(x.PagePath, x.Parameters));

            this.RefreshCommand = new AsyncReactiveCommand();
            this.RefreshCommand.Subscribe(this.RefreshAsync);
        }

        [Unity.Dependency]
        public IFetchEnvironmentUseCase FetchEnvironmentUseCase { get; set; }

        public AsyncReactiveCommand RefreshCommand { get; }

        public AsyncReactiveCommand<SettingItem> ItemTappedCommand { get; }

        public ReactiveCollection<SettingItem> SettingItems { get; } =
            new ReactiveCollection<SettingItem>
            {
                new SettingItem { Name = "ブロック中のユーザー", PagePath = "BlockUsersPage" },
                new SettingItem { Name = "利用規約", PagePath = "WebViewPage" },
                new SettingItem { Name = "プライバシーポリシー", PagePath = "WebViewPage" },
                new SettingItem { Name = "お問い合わせ", PagePath = "InquiryPage" },
                new SettingItem { Name = "バージョン情報", PagePath = "AboutPage" },
                new SettingItem { Name = "ライセンス", PagePath = "LicensePage" },
                new SettingItem { Name = "退会する", PagePath = "UnsubscribePage" },
            };

        private Task RefreshAsync()
        {
            return Observable
                .Defer(this.FetchEnvironmentUseCase.Invoke)
                .SubscribeOn(this.SchedulerProvider.IO)
                .ObserveOn(this.SchedulerProvider.UI)
                .Do(environment =>
                {
                    this.SettingItems[1].Parameters = new NavigationParameters
                    {
                        { ParameterNames.Url, environment.TermsUrl } ,
                        { ParameterNames.Title, "利用規約" }
                    };

                    this.SettingItems[2].Parameters = new NavigationParameters
                    {
                        { ParameterNames.Url, environment.PrivacyPolicyUrl } ,
                        { ParameterNames.Title, "プライバシーポリシー" }
                    };
                })
                .Select(_ => Unit.Default)
                .Catch(Observable.Return(Unit.Default))
                .ToTask();
        }

        public class SettingItem
        {
            public string Name { get; set; }

            public string PagePath { get; set; }

            public NavigationParameters Parameters { get; set; }
        }
    }
}
