#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Collections;
using System.Reactive.Linq;
using Capibara.Presentation.Navigation;
using Moq;
using NUnit.Framework;

namespace Capibara.Presentation.ViewModels
{
    [TestFixture]
    public class ImagePageViewModelTest
    {
        #region Initialize

        public static IEnumerable Initialize_TestCaseSource()
        {
            yield return new TestCaseData(string.Empty)
                .SetName("Initialize when url empty should ImageUrl is null");

            yield return new TestCaseData(null)
                .SetName("Initialize when url null should ImageUrl is null");


            yield return new TestCaseData(Faker.Url.Root())
                .SetName("Initialize when url present should ImageUrl is present");
        }

        [Test]
        [TestCaseSource("Initialize_TestCaseSource")]
        public void InitializeTest(string imageUrl)
        {
            var viewModel = new ImagePageViewModel();
            var navigationParameter = new NavigationParameterBuilder { Url = imageUrl }.Build();

            viewModel.Initialize(navigationParameter);

            Assert.That(viewModel.ImageUrl.Value, Is.EqualTo(imageUrl.Presence()));
        }

        #endregion
    }
}
