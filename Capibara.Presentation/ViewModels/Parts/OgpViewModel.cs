﻿using Capibara.Domain.Models;
using Capibara.Domain.UseCases;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.Presentation.ViewModels.Parts
{
    public class OgpViewModel : ViewModelBase<Ogp>
    {
        public OgpViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            Ogp model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.RefreshCommand = new AsyncReactiveCommand()
                .AddTo(this.Disposable);
            this.RefreshCommand
                .Subscribe(() => this.FetchOgpUseCase.Invoke(this.Model));

            this.OpenUrlCommand = new AsyncReactiveCommand()
                .AddTo(this.Disposable);
            this.OpenUrlCommand
                .Subscribe(() => this.OpenUrlUseCase.Invoke(this.Model.Url));

            this.Title =  this.Model
                .ToReactivePropertyAsSynchronized(x => x.Title)
                .AddTo(this.Disposable);

            this.Description = this.Model
                .ToReactivePropertyAsSynchronized(x => x.Description)
                .AddTo(this.Disposable);

            this.ImageUrl = this.Model
                .ToReactivePropertyAsSynchronized(x => x.ImageUrl)
                .AddTo(this.Disposable);
        }

        public IOpenUrlUseCase OpenUrlUseCase { get; set; }

        public IFetchOgpUseCase FetchOgpUseCase { get; set; }

        public AsyncReactiveCommand OpenUrlCommand { get; }

        public AsyncReactiveCommand RefreshCommand { get; }

        public ReactiveProperty<string> Title { get; }

        public ReactiveProperty<string> Description { get; }

        public ReactiveProperty<string> ImageUrl { get; }
    }
}