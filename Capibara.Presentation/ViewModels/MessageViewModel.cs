using System;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using Capibara.Domain.Models;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.Presentation.ViewModels
{
    public class MessageViewModel : ViewModelBase<Message>
    {
        private const string URLPattern = @"https?://[\w/:%#\$&\?\(\)~\.=\+\-]+";

        public MessageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            Message model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.Id = this.Model
                .ToReactivePropertyAsSynchronized(x => x.Id)
                .AddTo(this.Disposable);

            this.Content = this.Model
                .ToReactivePropertyAsSynchronized(x => x.Content)
                .AddTo(this.Disposable);

            this.At = this.Model
                .ToReactivePropertyAsSynchronized(x => x.At)
                .AddTo(this.Disposable);

            this.Sender = this.Model
                .ObserveProperty(x => x.Sender)
                .Select(x => new UserViewModel(navigationService, pageDialogService, x))
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.IconThumbnailUrl = this.Sender
                .SelectMany(x => x.IconThumbnailUrl)
                .ToReactiveProperty();

            this.ImageThumbnailUrl = this.Model
                .ToReactivePropertyAsSynchronized(x => x.ImageThumbnailUrl)
                .AddTo(this.Disposable);

            this.OgpItems = this.Content
                .SelectMany(x => Regex.Matches(x, URLPattern, RegexOptions.IgnoreCase).Cast<Match>())
                .Select(x => x.Value)
                .Select(x => new OgpViewModel(navigationService, pageDialogService, new Ogp { Url = x }))
                .ToReactiveCollection();
        }

        public ReactiveProperty<int> Id { get; }

        public ReactiveProperty<string> Content { get; }

        public ReactiveProperty<DateTimeOffset> At { get; }

        public ReactiveProperty<UserViewModel> Sender { get; }

        public ReactiveProperty<string> IconThumbnailUrl { get; }

        public ReactiveProperty<string> ImageThumbnailUrl { get; }

        public ReactiveCollection<OgpViewModel> OgpItems { get; }
    }
}
