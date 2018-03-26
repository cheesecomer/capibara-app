using Xamarin.Forms;

namespace Capibara.Forms
{
    public class LinkableLabel : Label
    {
        /// <summary>
        /// LinkTextColor Bindable プロパティ
        /// </summary>
        public static readonly BindableProperty LinkTextColorProperty = BindableProperty.Create(
            nameof(LinkTextColor), // プロパティ名
            typeof(Color), // プロパティの型
            typeof(LinkableLabel), // プロパティを持つ View の型
            Color.Default, // 初期値
            BindingMode.TwoWay // バインド方向
            );

        public Color LinkTextColor
        {
            get { return (Color)this.GetValue(LinkableLabel.LinkTextColorProperty); }
            set { this.SetValue(LinkableLabel.LinkTextColorProperty, value); }
        }
    }
}
