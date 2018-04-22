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
        public ReactiveProperty<bool> IsBlock { get; }

        public ReactiveProperty<string> ToggleFollowDescription { get; }

        public AsyncReactiveCommand BlockCommand { get; }

        public AsyncReactiveCommand ReportCommand { get; }

        public AsyncReactiveCommand ToggleFollowCommand { get; }

        public UserProfilePageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            User model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.ToggleFollowDescription = this.Model
                .ObserveProperty(x => x.IsFollow)
                .Select((x) => x ? "DM を拒否する" : "DM を受け付ける")
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.IsBlock = this.Model
                .ToReactivePropertyAsSynchronized(x => x.IsBlock)
                .AddTo(this.Disposable);
            this.IsBlock.Subscribe(_ => this.RaisePropertyChanged(nameof(this.IsBlock)));

            // BlockCommand
            this.BlockCommand = this.IsBlock.Select(x => !x).ToAsyncReactiveCommand().AddTo(this.Disposable);
            this.BlockCommand.Subscribe(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.Block()));

            this.Model.BlockFail += this.OnFail(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.Block()));

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
            this.ToggleFollowCommand = this.IsBlock.Select(x => !x).ToAsyncReactiveCommand().AddTo(this.Disposable);
            this.ToggleFollowCommand.Subscribe(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.ToggleFollow()));
            this.Model.ToggleFollowFail += this.OnFail(() => this.ProgressDialogService.DisplayProgressAsync(this.Model.ToggleFollow()));
        }
    }
}
