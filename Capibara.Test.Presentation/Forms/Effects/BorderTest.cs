#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Linq;
using NUnit.Framework;
using Xamarin.Forms;

namespace Capibara.Presentation.Forms.Effects
{
    public class BorderTest
    {
        [Test]
        public void SetWidth_WhenBeforeSet()
        {
            var view = new ContentView();

            Assert.That(view.Effects.Count(x => x is BorderRoutingEffect), Is.EqualTo(0));
        }

        [Test]
        public void SetWidth_WhenAfterSet()
        {
            var view = new ContentView();

            Border.SetWidth(view, 1);

            Assert.That(view.Effects.Count(x => x is BorderRoutingEffect), Is.EqualTo(1));
        }

        [Test]
        public void SetWidth_WhenMany()
        {
            var view = new ContentView();

            Border.SetWidth(view, 1);
            Border.SetWidth(view, 1);
            Border.SetWidth(view, 1);

            Assert.That(view.Effects.Count(x => x is BorderRoutingEffect), Is.EqualTo(1));
        }

        [Test]
        public void GetWidth_WhenBeforeSet()
        {
            var view = new ContentView();

            Assert.That(Border.GetWidth(view), Is.EqualTo(0d));
        }

        [Test]
        public void GetWidth_WhenAfterSet()
        {
            var view = new ContentView();

            Border.SetWidth(view, 1);

            Assert.That(Border.GetWidth(view), Is.EqualTo(1));
        }

        [Test]
        public void SetRadius_WhenBeforeSet()
        {
            var view = new ContentView();

            Assert.That(view.Effects.Count(x => x is BorderRoutingEffect), Is.EqualTo(0));
        }

        [Test]
        public void SetRadius_WhenAfterSet()
        {
            var view = new ContentView();

            Border.SetRadius(view, 1);

            Assert.That(view.Effects.Count(x => x is BorderRoutingEffect), Is.EqualTo(1));
        }

        [Test]
        public void SetRadius_WhenMany()
        {
            var view = new ContentView();

            Border.SetRadius(view, 1);
            Border.SetRadius(view, 1);
            Border.SetRadius(view, 1);

            Assert.That(view.Effects.Count(x => x is BorderRoutingEffect), Is.EqualTo(1));
        }

        [Test]
        public void GetRadius_WhenBeforeSet()
        {
            var view = new ContentView();

            Assert.That(Border.GetRadius(view), Is.EqualTo(0d));
        }

        [Test]
        public void GetRadius_WhenAfterSet()
        {
            var view = new ContentView();

            Border.SetRadius(view, 1);

            Assert.That(Border.GetRadius(view), Is.EqualTo(1));
        }

        [Test]
        public void SetColor_WhenBeforeSet()
        {
            var view = new ContentView();

            Assert.That(view.Effects.Count(x => x is BorderRoutingEffect), Is.EqualTo(0));
        }

        [Test]
        public void SetColor_WhenAfterSet()
        {
            var view = new ContentView();

            Border.SetColor(view, Color.Red);

            Assert.That(view.Effects.Count(x => x is BorderRoutingEffect), Is.EqualTo(1));
        }

        [Test]
        public void SetColor_WhenMany()
        {
            var view = new ContentView();

            Border.SetColor(view, Color.Red);
            Border.SetColor(view, Color.Red);
            Border.SetColor(view, Color.Red);

            Assert.That(view.Effects.Count(x => x is BorderRoutingEffect), Is.EqualTo(1));
        }

        [Test]
        public void GetColor_WhenBeforeSet()
        {
            var view = new ContentView();

            Assert.That(Border.GetColor(view), Is.EqualTo(Color.Transparent));
        }

        [Test]
        public void GetColor_WhenAfterSet()
        {
            var view = new ContentView();

            Border.SetColor(view, Color.Red);

            Assert.That(Border.GetColor(view), Is.EqualTo(Color.Red));
        }
    }
}
