using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Linq;

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
        public ReactiveCollection<ReportReason> ReportReasons { get; } = new ReactiveCollection<ReportReason>()
        {
            ReportReason.Spam,
            ReportReason.AbusiveOrHatefulSpeech,
            ReportReason.AbusiveOrHatefulImage,
            ReportReason.ObsceneSpeech,
            ReportReason.ObsceneImage,
            ReportReason.Other
        };

        public ReactiveProperty<int> SelectedIndex { get; } = new ReactiveProperty<int>();

        public ReactiveProperty<ReportReason> SelectedItem { get; } = new ReactiveProperty<ReportReason>();

        public ReactiveProperty<string> Message { get; } = new ReactiveProperty<string>();

        public AsyncReactiveCommand SubmitCommand { get; }

        public ReportPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            User model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.ReportReasons.AddTo(this.Disposable);
            this.SelectedItem.AddTo(this.Disposable);
            this.Message.AddTo(this.Disposable);

            this.SubmitCommand = 
                this.PropertyChangedAsObservable()
                .Select(_ => this.SelectedItem.Value != ReportReason.Other || this.Message.Value.ToSlim().IsPresent())
                .ToAsyncReactiveCommand()
                .AddTo(this.Disposable);
            this.SubmitCommand.Subscribe(
                () => this.ProgressDialogService.DisplayProgressAsync(
                    this.Model.Report(this.SelectedItem.Value, this.Message.Value)));

            this.SelectedIndex.Subscribe(x => this.SelectedItem.Value = this.ReportReasons.ElementAtOrDefault(x));
            this.SelectedIndex.Subscribe(_ => this.RaisePropertyChanged(nameof(SelectedIndex)));
            this.SelectedItem.Subscribe(_ => this.RaisePropertyChanged(nameof(SelectedItem)));
            this.Message.Subscribe(_ => this.RaisePropertyChanged(nameof(Message)));

            this.SelectedIndex.Value = 0;

            this.Model.ReportSuccess += this.OnReportSuccess;
            this.Model.ReportFail += this.OnFail;
        }

        private void OnReportSuccess(object sender, EventArgs args)
        {
            this.NavigationService.GoBackAsync();
        }
    }
}
