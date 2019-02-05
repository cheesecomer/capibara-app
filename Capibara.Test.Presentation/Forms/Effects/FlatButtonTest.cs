#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System.Linq;
using NUnit.Framework;
using Xamarin.Forms;

namespace Capibara.Presentation.Forms.Effects
{
    [TestFixture]
    public class FlatButtonTest
    {
        [Test]
        public void SetEnable_WhenBeforeSet()
        {
            var view = new Button();

            Assert.That(view.Effects.Count(x => x is FlatButtonRoutingEffect), Is.EqualTo(0));
        }

        [Test]
        public void SetEnable_WhenAfterSet()
        {
            var view = new Button();

            FlatButton.SetEnable(view, true);

            Assert.That(view.Effects.Count(x => x is FlatButtonRoutingEffect), Is.EqualTo(1));
        }

        [Test]
        public void SetEnable_WhenNotButton()
        {
            var view = new ContentView();

            FlatButton.SetEnable(view, true);

            Assert.That(view.Effects.Count(x => x is FlatButtonRoutingEffect), Is.EqualTo(0));
        }

        [Test]
        public void GetEnable_WhenBeforeSet()
        {
            var view = new Button();

            Assert.That(FlatButton.GetEnable(view), Is.EqualTo(false));
        }

        [Test]
        public void GetEnable_WhenAfterSet()
        {
            var view = new Button();

            FlatButton.SetEnable(view, true);

            Assert.That(FlatButton.GetEnable(view), Is.EqualTo(true));
        }

        [Test]
        public void SetRippleColor_WhenBeforeSet()
        {
            var view = new Button();

            Assert.That(view.Effects.Count(x => x is FlatButtonRoutingEffect), Is.EqualTo(0));
        }

        [Test]
        public void SetRippleColor_WhenAfterSet()
        {
            var view = new Button();

            FlatButton.SetRippleColor(view, Color.Red);

            Assert.That(view.Effects.Count(x => x is FlatButtonRoutingEffect), Is.EqualTo(0));
        }

        [Test]
        public void GetRippleColor_WhenBeforeSet()
        {
            var view = new Button();

            Assert.That(FlatButton.GetRippleColor(view), Is.EqualTo(default(Color)));
        }

        [Test]
        public void GetRippleColor_WhenAfterSet()
        {
            var view = new Button();

            FlatButton.SetRippleColor(view, Color.Red);

            Assert.That(FlatButton.GetRippleColor(view), Is.EqualTo(Color.Red));
        }
    }
}
