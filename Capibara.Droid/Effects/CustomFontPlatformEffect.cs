using System;
using Android.Graphics.Drawables;
using Capibara.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using System.ComponentModel;

[assembly: ExportEffect(typeof(Capibara.Droid.Effects.CustomFontPlatformEffect), nameof(Capibara.Effects.CustomFont))]
namespace Capibara.Droid.Effects
{
    public class CustomFontPlatformEffect : EffectBase
    {
        protected override void OnAttached()
        {
            this.OnFontFamilyChanged();
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if (args.PropertyName == "FontFamily")
            {
                this.OnFontFamilyChanged();
            }
        }

        protected override void OnDetached()
        {
        }

        private void OnFontFamilyChanged()
        {
            var fontfamily = this.Element?.GetType().GetProperty("FontFamily").GetValue(this.Element) as string;
            if (fontfamily.IsNullOrEmpty())
            {
                return;
            }
            try
            {
                var font = Typeface.CreateFromAsset(MainActivity.Instance.Assets, fontfamily);
                this.Control?.GetType().GetProperty("Typeface").SetValue(this.Control, font);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
