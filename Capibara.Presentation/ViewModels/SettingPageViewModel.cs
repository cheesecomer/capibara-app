using System;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

using Prism.Navigation;
using Prism.Services;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Capibara.Domain.UseCases;
using Capibara.Presentation.Navigation;

namespace Capibara.Presentation.ViewModels
{
    public class SettingPageViewModel : ViewModelBase
    {
        public SettingPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
            this.ItemTappedCommand =
                new AsyncReactiveCommand<SettingItem>()
                    .WithSubscribe(this.NavigationAsync)
                    .AddTo(this.Disposable);

            this.SettingItems = new ReactiveCollection<SettingItem>
                {
                    new SettingItem { Name = "ブロック中のユーザー", PagePath = "BlockUsersPage" },
                    new SettingItem
                    {
                        Name = "利用規約",
                        PagePath = "WebViewPage",
                        ObservableFactory = () =>
                        {
                            return GetWebPageUrlUseCase
                                .Invoke(Domain.Models.WebPage.Terms)
                                .Select(x => new NavigationParameterBuilder { Url = x, Title = "利用規約" }.Build())
                                ;
                        }
                    },
                    new SettingItem
                    {
                        Name = "プライバシーポリシー",
                        PagePath = "WebViewPage",
                        ObservableFactory = () =>
                        {
                            return GetWebPageUrlUseCase
                                .Invoke(Domain.Models.WebPage.PrivacyPolicy)
                                .Select(x => new NavigationParameterBuilder { Url = x, Title = "プライバシーポリシー" }.Build())
                                ;
                        }
                    },
                    new SettingItem { Name = "お問い合わせ", PagePath = "InquiryPage" },
                    new SettingItem { Name = "バージョン情報", PagePath = "AboutPage" },
                    new SettingItem { Name = "ライセンス", PagePath = "LicensePage" },
                    new SettingItem { Name = "退会する", PagePath = "UnsubscribePage" },
                };
        }

        [Unity.Dependency]
        public IGetWebPageUrlUseCase GetWebPageUrlUseCase { get; set; }

        public AsyncReactiveCommand<SettingItem> ItemTappedCommand { get; }

        public ReactiveCollection<SettingItem> SettingItems { get; } 

        private Task NavigationAsync(SettingItem item)
        {
            var observableFactory = item.ObservableFactory ?? new Func<IObservable<NavigationParameters>>(() => Observable.Return<NavigationParameters>(null));

            return Observable.Defer(observableFactory.Invoke)
                .SubscribeOn(this.SchedulerProvider.IO)
                .SelectMany(x => this.NavigationService.NavigateAsync(item.PagePath, x))
                .SubscribeOn(this.SchedulerProvider.UI)
                .ToTask();
        }

        public class SettingItem
        {
            public string Name { get; set; }

            public string PagePath { get; set; }

            public NavigationParameters Parameters { get; set; }

            public Func<IObservable<NavigationParameters>> ObservableFactory { get; set; }
        }
    }
}
