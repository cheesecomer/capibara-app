using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;

using Capibara.Models;

using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Xamarin.Forms;

namespace Capibara.ViewModels
{
    public class DirectMessageViewModel : ViewModelBase<DirectMessage>
    {
        public ReactiveProperty<int> Id { get; }

        public ReactiveProperty<string> Content { get; }

        public ReactiveProperty<bool> IsOwn { get; }

        public ReactiveProperty<DateTimeOffset> At { get; }

        public ReactiveProperty<UserViewModel> Sender { get; }

        public ReactiveProperty<List<OgpViewModel>> OgpItems { get; }

        public ReactiveProperty<ImageSource> IconThumbnail { get; } = new ReactiveProperty<ImageSource>();

        public AsyncReactiveCommand ShowProfileCommand { get; }

        public DirectMessageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            DirectMessage model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.Id = this.Model
                .ObserveProperty(x => x.Id)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.Content = this.Model
                .ObserveProperty(x => x.Content)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.At = this.Model
                .ObserveProperty(x => x.At)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.Sender = this.Model
                .ObserveProperty(x => x.Sender)
                .Select(x => new UserViewModel(navigationService, pageDialogService, x))
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.IsOwn = this.Model
                .ObserveProperty(x => x.IsOwn)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.Model.Sender
                .ObserveProperty(x => x.IconThumbnailUrl)
                .Subscribe(x => this.IconThumbnail.Value = x);
            
            // ShowProfileCommand
            this.ShowProfileCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.ShowProfileCommand.Subscribe(() =>
            {
                var parameters = new NavigationParameters();
                parameters.Add(ParameterNames.Model, this.Sender.Value.Model);
                return this.NavigationService.NavigateAsync("UserProfilePage", parameters);
            });

            var pattern = @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
            this.OgpItems = this.Content
                .Select(x => Regex.Matches(x, pattern, RegexOptions.IgnoreCase).Cast<Match>())
                .Select(x => x.Select(v => new OgpViewModel(navigationService, pageDialogService, v.Value)))
                .Select(x => x.Select(v => this.Container.IsNull() ? v : v.BuildUp(this.Container)).ToList())
                .ToReactiveProperty();

            // OGP が変更された場合は読み込みコマンドを実行する
            this.OgpItems.Subscribe(x => x.ForEach(v => v.RefreshCommand.Execute()));
        }

        protected override void OnContainerChanged()
        {
            base.OnContainerChanged();

            this.OgpItems.Value.ForEach(x => x.BuildUp(this.Container));
        }
    }
}
