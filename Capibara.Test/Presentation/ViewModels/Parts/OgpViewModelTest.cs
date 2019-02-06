#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using Reactive.Bindings.Extensions;
using Moq;
using Capibara.Domain.Models;
using Capibara.Domain.UseCases;

namespace Capibara.Presentation.ViewModels.Parts
{
    public class OgpViewModelTest
    {
        #region TitleProperty

        [Test]
        public void TitleProperty_WhenModelChanged_ShouldUpdate()
        {
            var subject = new OgpViewModel();
            var expected = subject.Model.Title = Faker.Lorem.Sentence();
            Assert.That(subject.Title.Value, Is.EqualTo(expected));
        }

        [Test]
        public void TitleProperty_WhenModelNotChanged_ShouldNotRaisePropertyChanged()
        {
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<PropertyChangedEventArgs>();
            var subject = new OgpViewModel();
            subject.Title
                .PropertyChangedAsObservable()
                .Where(x => x.PropertyName == nameof(subject.Title.Value))
                .Subscribe(observer);
            Assert.That(observer.Messages.Count, Is.EqualTo(0));
        }

        [Test]
        public void TitleProperty_WhenModelChanged_ShouldRaisePropertyChanged()
        {
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<PropertyChangedEventArgs>();
            var subject = new OgpViewModel();
            subject.Title
                .PropertyChangedAsObservable()
                .Where(x => x.PropertyName == nameof(subject.Title.Value))
                .Subscribe(observer);
            subject.Model.Title = Faker.Lorem.Sentence();
            Assert.That(observer.Messages.Count, Is.EqualTo(1));
        }

        #endregion

        #region DescriptionProperty

        [Test]
        public void DescriptionProperty_WhenModelChanged_ShouldUpdate()
        {
            var subject = new OgpViewModel();
            var expected = subject.Model.Description = Faker.Lorem.Paragraph();
            Assert.That(subject.Description.Value, Is.EqualTo(expected));
        }

        [Test]
        public void DescriptionProperty_WhenModelNotChanged_ShouldNotRaisePropertyChanged()
        {
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<PropertyChangedEventArgs>();
            var subject = new OgpViewModel();
            subject.Description
                .PropertyChangedAsObservable()
                .Where(x => x.PropertyName == nameof(subject.Description.Value))
                .Subscribe(observer);
            Assert.That(observer.Messages.Count, Is.EqualTo(0));
        }

        [Test]
        public void DescriptionProperty_WhenModelChanged_ShouldRaisePropertyChanged()
        {
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<PropertyChangedEventArgs>();
            var subject = new OgpViewModel();
            subject.Description
                .PropertyChangedAsObservable()
                .Where(x => x.PropertyName == nameof(subject.Description.Value))
                .Subscribe(observer);
            subject.Model.Description = Faker.Lorem.Paragraph();
            Assert.That(observer.Messages.Count, Is.EqualTo(1));
        }

        #endregion

        #region ImageUrlProperty

        [Test]
        public void ImageUrlProperty_WhenModelChanged_ShouldUpdate()
        {
            var subject = new OgpViewModel();
            var expected = subject.Model.ImageUrl = Faker.Url.Image();
            Assert.That(subject.ImageUrl.Value, Is.EqualTo(expected));
        }

        [Test]
        public void ImageUrlProperty_WhenModelNotChanged_ShouldNotRaisePropertyChanged()
        {
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<PropertyChangedEventArgs>();
            var subject = new OgpViewModel();
            subject.ImageUrl
                .PropertyChangedAsObservable()
                .Where(x => x.PropertyName == nameof(subject.ImageUrl.Value))
                .Subscribe(observer);
            Assert.That(observer.Messages.Count, Is.EqualTo(0));
        }

        [Test]
        public void ImageUrlProperty_WhenModelChanged_ShouldRaisePropertyChanged()
        {
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<PropertyChangedEventArgs>();
            var subject = new OgpViewModel();
            subject.ImageUrl
                .PropertyChangedAsObservable()
                .Where(x => x.PropertyName == nameof(subject.ImageUrl.Value))
                .Subscribe(observer);
            subject.Model.ImageUrl = Faker.Url.Image();
            Assert.That(observer.Messages.Count, Is.EqualTo(1));
        }

        #endregion

        #region OpenUrlCommand

        [Test]
        public void OpenUrlCommand_ShouldInvokeFetchOgp()
        {
            var openUrlUseCase = new Mock<IOpenUrlUseCase>();
            var model = ModelFixture.Ogp();
            new OgpViewModel(model: model) { OpenUrlUseCase = openUrlUseCase.Object }.OpenUrlCommand.Execute();
            openUrlUseCase.Verify(x => x.Invoke(model.Url), Times.Once);
        }

        #endregion

        #region RefreshCommand

        [Test]
        public void RefreshCommand_ShouldInvokeFetchOgp()
        {
            var fetchOgpUseCase = new Mock<IFetchOgpUseCase>();
            var model = ModelFixture.Ogp();
            new OgpViewModel(model: model) { FetchOgpUseCase = fetchOgpUseCase.Object }.RefreshCommand.Execute();
            fetchOgpUseCase.Verify(x => x.Invoke(model), Times.Once);
        }

        #endregion
    }
}
