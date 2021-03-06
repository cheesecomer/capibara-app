﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;

using Capibara.Models;

using Prism.Services;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.ViewModels
{
    public class InformationsPageViewModel : ViewModelBase
    {
        public ReactiveCollection<Information> Informations { get; } = new ReactiveCollection<Information>();

        public AsyncReactiveCommand RefreshCommand { get; }

        public AsyncReactiveCommand<Information> ItemTappedCommand { get; }

        public InformationsPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
            // RefreshCommand
            this.RefreshCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.RefreshCommand.Subscribe(() => this.ProgressDialogService.DisplayProgressAsync(this.Refresh()));

            this.ItemTappedCommand = new AsyncReactiveCommand<Information>();
            this.ItemTappedCommand.Subscribe(async x =>
            {
                var parameters = new NavigationParameters { 
                    { ParameterNames.Url, x.Url } ,
                    { ParameterNames.Title, "お知らせ" }
                };
                await this.NavigationService.NavigateAsync("WebViewPage", parameters);
            });
        }

        private async Task Refresh()
        {
            var request = this.RequestFactory.InformationsIndexRequest().BuildUp(this.Container);
            try
            {
                var response = await request.Execute();
                response.Informations?.ForEach(x => {
                    if (this.Informations.Any(y => y.Id == x.Id))
                    {
                        this.Informations.First(y => y.Id == x.Id).Restore(x);
                    }
                    else
                    {
                        this.Informations.Add(x);
                    }
                });

            }
            catch (Exception e)
            {
                if (await this.DisplayErrorAlertAsync(e))
                {
                    await this.Refresh();
                }
            }
        }
    }
}
