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
            new ReactiveCollection<SettingItem>();

        public AsyncReactiveCommand<SettingItem> ItemTappedCommand { get; }

        public SettingPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
            this.SettingItems.Add(new SettingItem { Name = "ブロック中のユーザー", PagePath = "BlockUsersPage" });
            this.SettingItems.Add(new SettingItem { Name = "退会する", PagePath = "UnsubscribePage" });

            this.ItemTappedCommand = new AsyncReactiveCommand<SettingItem>();
            this.ItemTappedCommand.Subscribe(async x => await this.NavigationService.NavigateAsync(x.PagePath));
        }

        public class SettingItem
        {
            public string Name { get; set; }

            public string PagePath { get; set; }
        }
    }
}
