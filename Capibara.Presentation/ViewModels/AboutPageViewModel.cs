using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Capibara.Domain.UseCases;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.Presentation.ViewModels
{
    public class AboutPageViewModel : ViewModelBase
    {
        public AboutPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
            this.OpenCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.OpenCommand.Subscribe(this.OpenAsync);

            this.CloseCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.CloseCommand.Subscribe(this.NavigationService.GoBackAsync);

            this.Copyright = new ReactiveProperty<string>().AddTo(this.Disposable);
            this.Copyright.Value = $"Copyright © 2018-{DateTime.Now.Year} @cheese_comer.";

            this.Version = new ReactiveProperty<string>().AddTo(this.Disposable);
        }

        [Unity.Dependency]
        public IGetApplicationVersionUseCase GetApplicationVersionUseCase { get; set; }

        public ReactiveProperty<string> Version { get; }

        public ReactiveProperty<string> Copyright { get; }

        public AsyncReactiveCommand OpenCommand { get; }

        public AsyncReactiveCommand CloseCommand { get; }

        private Task OpenAsync()
        {
            return Observable
                .FromAsync(this.GetApplicationVersionUseCase.Invoke, this.SchedulerProvider.IO)
                .SelectMany(x => 
                    Observable.Start(
                        () => this.Version.Value = $"Version {x}", 
                        this.SchedulerProvider.UI))
                .ToTask();
        }
    }
}
