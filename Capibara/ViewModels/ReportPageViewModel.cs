using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.Services;

using Prism.Navigation;
using Prism.Services;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Xamarin.Forms;

namespace Capibara.ViewModels
{
    public class ReportPageViewModel : ViewModelBase<User>
    {
        public ReactiveCollection<ReportReason> ReportReasons { get; } = new ReactiveCollection<ReportReason>();

        public ReactiveProperty<ReportReason> SelectedItem { get; } = new ReactiveProperty<ReportReason>();

        public ReactiveProperty<string> Message { get; } = new ReactiveProperty<string>();

        public AsyncReactiveCommand ReportCommand { get; }

        public ReportPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            User model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.ReportReasons.Add(ReportReason.Spam);
            this.ReportReasons.Add(ReportReason.AbusiveOrHatefulSpeech);
            this.ReportReasons.Add(ReportReason.AbusiveOrHatefulImage);
            this.ReportReasons.Add(ReportReason.ObsceneSpeech);
            this.ReportReasons.Add(ReportReason.ObsceneImage);
            this.ReportReasons.Add(ReportReason.Other);

            this.ReportCommand = 
                this.PropertyChangedAsObservable()
                .Select(_ => this.SelectedItem.Value != ReportReason.Other || this.Message.Value.ToSlim().IsPresent())
                .ToAsyncReactiveCommand()
                .AddTo(this.Disposable);
            this.ReportCommand.Subscribe(
                () => this.ProgressDialogService.DisplayProgressAsync(
                    this.Model.Report(this.SelectedItem.Value, this.Message.Value)));

            this.SelectedItem.Value = ReportReason.Spam;

            this.SelectedItem.Subscribe(_ => this.RaisePropertyChanged(nameof(SelectedItem)));
            this.Message.Subscribe(_ => this.RaisePropertyChanged(nameof(Message)));

            this.Model.ReportSuccess += this.OnReportSuccess;
        }

        private void OnReportSuccess(object sender, EventArgs args)
        {
            this.NavigationService.GoBackAsync();
        }
    }
}
