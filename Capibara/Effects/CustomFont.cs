using System.Linq;
using Xamarin.Forms;

namespace Capibara.Effects
{
    public static class CustomFont
    {
        private static void OnApply(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as View;
            if (view == null)
                return;

            var attachedEffect = view.Effects.FirstOrDefault(e => e is CustomFontRoutingEffect);
            if (attachedEffect == null)
            {
                //view.Effects.Remove(attachedEffect);
                view.Effects.Add(new CustomFontRoutingEffect());
            }

        }

        public static readonly BindableProperty OnProperty =
            BindableProperty.CreateAttached(
                "On",
                typeof(bool),
                typeof(CustomFont),
                false,
                propertyChanged: OnApply
            );

        public static void SetOn(BindableObject view, bool value)
        {
            view.SetValue(OnProperty, value);
        }

        public static bool GetOn(BindableObject view)
        {
            return (bool)view.GetValue(OnProperty);
        }

        class CustomFontRoutingEffect : RoutingEffect
        {
            public CustomFontRoutingEffect() : base("Capibara.Effects." + nameof(CustomFont)) { }
        }
    }
}
