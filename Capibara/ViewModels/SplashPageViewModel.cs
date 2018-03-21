﻿using System.Threading.Tasks;

using Capibara.Models;

using Prism.AppModel;
using Prism.Navigation;
using Prism.Services;

using Reactive.Bindings;

namespace Capibara.ViewModels
{
    public class SplashPageViewModel : ViewModelBase
    {
        public ReactiveProperty<double> LogoTopMargin { get; }
            = new ReactiveProperty<double>(180);

        public ReactiveProperty<double> LogoScale { get; }
            = new ReactiveProperty<double>(1);

        public ReactiveProperty<double> LogoOpacity { get; }
            = new ReactiveProperty<double>(1);

        public AsyncReactiveCommand RefreshCommand { get; }

        public SplashPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
            this.RefreshCommand = new AsyncReactiveCommand();
            this.RefreshCommand.Subscribe(this.RefreshAsync);
        }

        protected Task RefreshAsync()
        {
            if (this.IsolatedStorage.AccessToken.IsNullOrEmpty())
            {
                return this.ToSignUpPage();
            }
            else
            {
                return this.ToFloorMapPage();
            }
        }

        private async Task ToSignUpPage()
        {
            var millisecondPerFrame = 10;
            while (this.LogoTopMargin.Value > 20)
            {
                await this.TaskService.Delay(millisecondPerFrame);
                this.LogoTopMargin.Value -= (180d - 20d) / (500d / (double)millisecondPerFrame);
            }

            this.LogoTopMargin.Value = 20;

            await this.NavigationService.NavigateAsync("SignUpPage", animated: false);
        }

        private async Task ToFloorMapPage()
        {
            var user = new User { Id = this.IsolatedStorage.UserId }.BuildUp(this.Container);
            if (!await user.Refresh())
            {
                await this.ToSignUpPage();
            }
            else if (user.IsAccepted)
            {
                await Task.WhenAll(this.LogoOpacityChangeAsync(), this.LogoScaleChangeAsync());

                await this.NavigationService.NavigateAsync("/MainPage/NavigationPage/FloorMapPage", animated: false);
            }
            else
            {
                await Task.WhenAll(this.LogoOpacityChangeAsync(), this.LogoScaleChangeAsync());

                await this.NavigationService.NavigateAsync("/AcceptPage", new NavigationParameters { { ParameterNames.Model, user } }, animated: false);
            }
        }

        private async Task LogoScaleChangeAsync()
        {
            var millisecondPerFrame = 10;
            while (this.LogoScale.Value < 3)
            {
                await this.TaskService.Delay(millisecondPerFrame);
                this.LogoScale.Value += 2 / (500d / (double)millisecondPerFrame);
            }

            this.LogoScale.Value = 3;
        }

        private async Task LogoOpacityChangeAsync()
        {
            var millisecondPerFrame = 10;
            while (this.LogoOpacity.Value > 0)
            {
                await this.TaskService.Delay(millisecondPerFrame);
                this.LogoOpacity.Value -= 1 / (500d / (double)millisecondPerFrame);
            }

            this.LogoOpacity.Value = 0;
        }
    }
}
