using System;
using System.Linq;
using System.Reactive.Linq;

using Capibara.Models;

using Prism.Navigation;
using Prism.Services;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.ViewModels
{
    public class UserProfilePageViewModel : UserViewModel
    {
        public ReadOnlyReactiveProperty<bool> IsFollow { get; }

        public ReactiveProperty<string> ToggleFollowDescription { get; }

        public ReactiveProperty<string> ToggleBlockDescription { get; }

        public AsyncReactiveCommand ToggleBlockCommand { get; }

        public AsyncReactiveCommand ReportCommand { get; }

        public AsyncReactiveCommand ToggleFollowCommand { get; }

        public AsyncReactiveCommand ShowDirectMessageCommand { get; }

        public UserProfilePageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            User model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.ToggleFollowDescription = this.Model
                .ObserveProperty(x => x.IsFollow)
                .Select((x) => x ? "DM を受け付けています" : "DM を受け付ける")
                .ToReactiveProperty()
                .AddTo(this.Disposable);
            
            this.ToggleBlockDescription = this.Model
                .ObserveProperty(x => x.IsBlock)
                .Select((x) => x ? "ブロック中" : "ブロック")
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.IsFollow = this.Model
                .ObserveProperty(x => x.IsFollow)
                .ToReadOnlyReactiveProperty()
                .AddTo(this.Disposable);
            
            // ToggleBlockCommand
            this.ToggleBlockCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.ToggleBlockCommand.Subscribe(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.ToggleBlock()));
            this.Model.ToggleBlockFail += this.OnFail(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.ToggleBlock()));

            // ReportCommand
            this.ReportCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.ReportCommand.Subscribe(() =>
            {
                var parameters = new NavigationParameters();
                parameters.Add(ParameterNames.Model, this.Model);
                return this.NavigationService.NavigateAsync("ReportPage", parameters);
            });

            // ToggleFollowCommand
            //  ブロックしていない場合に実行可能
            this.ToggleFollowCommand = this.Model
                .ObserveProperty(x => x.IsBlock)
                .Select(x => !x)
                .ToAsyncReactiveCommand()
                .AddTo(this.Disposable);
            this.ToggleFollowCommand.Subscribe(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.ToggleFollow()));
            this.Model.ToggleFollowFail += this.OnFail(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.ToggleFollow()));

            // ShowDirectMessageCommand
            this.ShowDirectMessageCommand = this.Model.ObserveProperty(x => x.IsFollow).ToAsyncReactiveCommand().AddTo(this.Disposable);
            this.ShowDirectMessageCommand.Subscribe(async () =>
            {
                var parameters = new NavigationParameters { { ParameterNames.Model, new DirectMessageThread { User = this.Model } } };
                await this.NavigationService.NavigateAsync("DirectMessagePage", parameters);
            });
        }
    }
}
