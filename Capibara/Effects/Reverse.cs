using System.Linq;
using Xamarin.Forms;

namespace Capibara.Effects
{
    public class Reverse
    {
        private static void OnApply(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as View;
            if (view == null)
                return;

            var attachedEffect = view.Effects.FirstOrDefault(e => e is ReverseRoutingEffect);
            if (attachedEffect == null)
            {
                //view.Effects.Remove(attachedEffect);
                view.Effects.Add(new ReverseRoutingEffect());
            }

        }

        public static readonly BindableProperty OnProperty =
            BindableProperty.CreateAttached(
                "On",
                typeof(bool),
                typeof(Reverse),
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

        class ReverseRoutingEffect : RoutingEffect
        {
            public ReverseRoutingEffect() : base("Capibara.Effects." + nameof(Reverse)) { }
        }
    }
}
