using System.Linq;
using Xamarin.Forms;

namespace Capibara.Presentation.Forms.Effects
{
    public class BorderRoutingEffect : RoutingEffect
    {
        public BorderRoutingEffect() : base("Capibara.Effects." + nameof(Border)) { }
    }

    public static class Border
    {
        #region BindableProperty
        public static readonly BindableProperty RadiusProperty =
            BindableProperty.CreateAttached(
                "Radius",
                typeof(double),
                typeof(Border),
                default(double),
                propertyChanged: OnApply);

        public static readonly BindableProperty WidthProperty =
            BindableProperty.CreateAttached(
                "Width",
                typeof(double),
                typeof(Border),
                default(double),
                propertyChanged: OnApply);

        public static readonly BindableProperty ColorProperty =
            BindableProperty.CreateAttached(
                "Color",
                typeof(Color),
                typeof(Border),
                Color.Transparent,
                propertyChanged: OnApply);
        #endregion

        public static void SetRadius(BindableObject view, double value)
        {
            view.SetValue(RadiusProperty, value);
        }

        public static double GetRadius(BindableObject view)
        {
            return (double)view.GetValue(RadiusProperty);
        }

        public static void SetWidth(BindableObject view, double value)
        {
            view.SetValue(WidthProperty, value);
        }

        public static double GetWidth(BindableObject view)
        {
            return (double)view.GetValue(WidthProperty);
        }

        public static void SetColor(BindableObject view, Color value)
        {
            view.SetValue(ColorProperty, value);
        }

        public static Color GetColor(BindableObject view)
        {
            return (Color)view.GetValue(ColorProperty);
        }

        private static void OnApply(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is View view)
            {
                if (view == null)
                    return;

                var attachedEffect = view.Effects.FirstOrDefault(e => e is BorderRoutingEffect);
                if (attachedEffect == null)
                {
                    view.Effects.Add(new BorderRoutingEffect());
                }
            }
        }
    }
}
