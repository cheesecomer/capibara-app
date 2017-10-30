using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Xamarin.Forms;

namespace Capibara.ViewModels
{
    public class SplashPageViewModel : BindableBase, INavigationAware
    {
        private readonly INavigationService navigationService;

        public ReactiveProperty<bool> IsPlaying { get; } = new ReactiveProperty<bool>();

        public SplashPageViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;

            //var timer = new ReactiveTimer(TimeSpan.FromSeconds(3));
            //timer.Subscribe(async _ =>
            //    {
            //        timer.Stop();
            //        Console.WriteLine("Go to FloorMapPage");
            //        await DeviceUtil.BeginInvokeOnMainThreadAsync(async () => await this.navigationService.NavigateAsync("/FloorMapPage", null, false, false));
            //        Console.WriteLine("Forwarded FloorMapPage");
            //    });
            //timer.Start(TimeSpan.FromSeconds(3));

            this.IsPlaying
                .Where(x => !x)
                .Delay(TimeSpan.FromSeconds(3))
                .ObserveOnUIDispatcher()
                .Subscribe(async _ =>
                {
                    Console.WriteLine("Go to FloorMapPage");
                    await this.navigationService.NavigateAsync("/FloorMapPage", null, false, false);
                    Console.WriteLine("Forwarded FloorMapPage");
                });
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {

        }
    }
}
