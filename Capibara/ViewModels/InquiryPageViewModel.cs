using System;
using System.Threading.Tasks;
using System.Reactive.Linq;

using Prism.Navigation;
using Prism.Services;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Xamarin.Forms;

namespace Capibara.ViewModels
{
    public class InquiryPageViewModel : ViewModelBase
    {
        public ReactiveProperty<string> Email { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> Message { get; } = new ReactiveProperty<string>();

        public AsyncReactiveCommand SubmitCommand { get; }

        public InquiryPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
            this.Email.Subscribe(_ => this.RaisePropertyChanged(nameof(Email)));
            this.Message.Subscribe(_ => this.RaisePropertyChanged(nameof(Message)));

            this.SubmitCommand = this.PropertyChangedAsObservable()
                .Select(_ => this.Email.Value.ToSlim().IsPresent() && this.Message.Value.ToSlim().IsPresent())
                .ToAsyncReactiveCommand()
                .AddTo(this.Disposable);
            this.SubmitCommand.Subscribe(() => this.ProgressDialogService.DisplayProgressAsync(this.Submit()));
        }

        private async Task Submit()
        {
            var request = this.RequestFactory.InquiriesCreateRequest(this.Email.Value, this.Message.Value).BuildUp(this.Container);
            try
            {
                await request.Execute();
                await this.NavigationService.GoBackAsync();
            }
            catch (Exception e)
            {
                await this.DisplayErrorAlertAsync(e, () => this.Submit());
            }
        }
    }
}
