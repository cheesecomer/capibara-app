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
    public class SettingPageViewModel : ViewModelBase
    {
        public ReactiveCollection<SettingItem> SettingItems { get; } =
            new ReactiveCollection<SettingItem>
            {
            new SettingItem { Name = "ブロック中のユーザー", PagePath = "BlockUsersPage" },
            new SettingItem { Name = "プライバシーポリシー", PagePath = "WebViewPage" },
            new SettingItem { Name = "退会する", PagePath = "UnsubscribePage" },
            };

        public AsyncReactiveCommand<SettingItem> ItemTappedCommand { get; }

        public SettingPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
            this.ItemTappedCommand = new AsyncReactiveCommand<SettingItem>();
            this.ItemTappedCommand.Subscribe(async x => await this.NavigationService.NavigateAsync(x.PagePath, x.Parameters));
        }

        protected override void OnContainerChanged()
        {
            base.OnContainerChanged();

            this.SettingItems[1].Parameters = new NavigationParameters
            { 
                { ParameterNames.Url, this.Environment.PrivacyPolicyUrl } ,
                { ParameterNames.Title, "プライバシーポリシー" }
            };
        }

        public class SettingItem
        {
            public string Name { get; set; }

            public string PagePath { get; set; }

            public NavigationParameters Parameters { get; set; }
        }
    }
}
