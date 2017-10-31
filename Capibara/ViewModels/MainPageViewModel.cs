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

        public ReactiveProperty<string> Nickname { get; }

        public MainPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
            this.MenuItems.Add(new MenuItem() { Name = "プロフィール" });
            this.MenuItems.Add(new MenuItem() { Name = "ホーム" });
            this.MenuItems.Add(new MenuItem() { Name = "設定" });
            this.MenuItems.Add(new MenuItem() { Name = "お問い合わせ" });
        }

        public class MenuItem
        {
            public string Name { get; set; }
        }
    }
}
