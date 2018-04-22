using System;
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

        public AsyncReactiveCommand BlockCommand { get; }

        public AsyncReactiveCommand ReportCommand { get; }

        public UserProfilePageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            User model = null)
            : base(navigationService, pageDialogService, model)
        {

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
        }
    }
}
