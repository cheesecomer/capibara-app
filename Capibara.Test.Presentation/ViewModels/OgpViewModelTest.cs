#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Capibara.Domain.Models;
using Capibara.Domain.UseCases;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using Reactive.Bindings.Extensions;

namespace Capibara.Presentation.ViewModels.Parts
{
    [TestFixture]
    public class OgpViewModelTest
    {
        static private IEnumerable Property_WhenRisePropertyChanged_ShouldUpdate_TestCase()
        {
            yield return new TestCaseData(
                new Func<OgpViewModel, object>(x => x.Model.Title = Faker.Lorem.Sentence()),
                new Func<OgpViewModel, object>(x => x.Title.Value))
                .SetName("Title Property When changed Should change");

            yield return new TestCaseData(
                new Func<OgpViewModel, object>(x => x.Model.Description = Faker.Lorem.Paragraph()),
                new Func<OgpViewModel, object>(x => x.Description.Value))
                .SetName("Description Property When changed Should change");

            yield return new TestCaseData(
                new Func<OgpViewModel, object>(x => x.Model.ImageUrl = Faker.Url.Image()),
                new Func<OgpViewModel, object>(x => x.ImageUrl.Value))
                .SetName("ImageUrl Property When changed Should change");
        }

        [Test]
        [TestCaseSource("Property_WhenRisePropertyChanged_ShouldUpdate_TestCase")]
        public void Property_WhenRisePropertyChanged_ShouldUpdate(Func<OgpViewModel, object> setter, Func<OgpViewModel, object> getter)
        {
            var subject = new OgpViewModel();
            var expected = setter(subject);
            Assert.That(getter(subject), Is.EqualTo(expected));
        }

        static private IEnumerable PropertyChangedTestCase()
        {
            yield return new TestCaseData(
                new Func<OgpViewModel, IObservable<PropertyChangedEventArgs>>(x => x.Title.PropertyChangedAsObservable()),
                new Action<Ogp>(x => { }),
                0)
                .SetName("Title Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                new Func<OgpViewModel, IObservable<PropertyChangedEventArgs>>(x => x.Title.PropertyChangedAsObservable()), 
                new Action<Ogp>(x => x.Title = Faker.Lorem.Sentence()), 1)
                .SetName("Title Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                new Func<OgpViewModel, IObservable<PropertyChangedEventArgs>>(x => x.Description.PropertyChangedAsObservable()), 
                new Action<Ogp>(x => { }), 0)
                .SetName("Description Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                new Func<OgpViewModel, IObservable<PropertyChangedEventArgs>>(x => x.Description.PropertyChangedAsObservable()), 
                new Action<Ogp>(x => x.Description = Faker.Lorem.Paragraph()), 1)
                .SetName("Description Property When chnaged Should raise PropertyChanged");

            yield return new TestCaseData(
                new Func<OgpViewModel, IObservable<PropertyChangedEventArgs>>(x => x.ImageUrl.PropertyChangedAsObservable()), 
                new Action<Ogp>(x => { }), 0)
                .SetName("ImageUrl Property When not chnaged Should not raise PropertyChanged");

            yield return new TestCaseData(
                new Func<OgpViewModel, IObservable<PropertyChangedEventArgs>>(x => x.ImageUrl.PropertyChangedAsObservable()), 
                new Action<Ogp>(x => x.ImageUrl = Faker.Url.Image()), 1)
                .SetName("ImageUrl Property When chnaged Should raise PropertyChanged");
        }

        [Test]
        [TestCaseSource("PropertyChangedTestCase")]
        public void PropertyChanged(
            Func<OgpViewModel, IObservable<PropertyChangedEventArgs>> observableGetter, 
            Action<Ogp> setter, 
            int expected)
        {
            var scheduler = new TestScheduler();
            var observer = scheduler.CreateObserver<PropertyChangedEventArgs>();
            var subject = new OgpViewModel();

            observableGetter(subject)
                .Where(x => x.PropertyName == "Value")
                .Subscribe(observer);

            setter(subject.Model);

            Assert.That(observer.Messages.Count, Is.EqualTo(expected));
        }

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
