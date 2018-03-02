using System;

using Xamarin.Forms;

namespace Capibara.Forms
{
    public class RoundImage : Image
    {
        /// <summary>
        /// CornerRadius Bindable プロパティ
        /// </summary>
        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
            nameof(CornerRadius), // プロパティ名
            typeof(float), // プロパティの型
            typeof(RoundImage), // プロパティを持つ View の型
            0f, // 初期値
            BindingMode.TwoWay // バインド方向
            );

        /// <summary>
        /// BorderColor Bindable プロパティ
        /// </summary>
        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
            nameof(BorderColor), // プロパティ名
            typeof(Color), // プロパティの型
            typeof(RoundImage), // プロパティを持つ View の型
            Color.Transparent, // 初期値
            BindingMode.TwoWay // バインド方向
            );
        
        /// <summary>
        /// BorderWidth Bindable プロパティ
        /// </summary>
        public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create(
            nameof(BorderWidth), // プロパティ名
            typeof(float), // プロパティの型
            typeof(RoundImage), // プロパティを持つ View の型
            0f, // 初期値
            BindingMode.TwoWay // バインド方向
            );

        /// <summary>
        /// BorderWidth Bindable プロパティ
        /// </summary>
        public static readonly BindableProperty HasShadowProperty = BindableProperty.Create(
            nameof(HasShadow), // プロパティ名
            typeof(bool), // プロパティの型
            typeof(RoundImage), // プロパティを持つ View の型
            false, // 初期値
            BindingMode.TwoWay // バインド方向
            );

        public float CornerRadius
        {
            get => (float)this.GetValue(RoundImage.CornerRadiusProperty);
            set => this.SetValue(RoundImage.CornerRadiusProperty, value);
        }

        public Color BorderColor
        {
            get => (Color)this.GetValue(RoundImage.BorderColorProperty);
            set => this.SetValue(RoundImage.BorderColorProperty, value);
        }

        public float BorderWidth
        {
            get => (float)this.GetValue(RoundImage.BorderWidthProperty);
            set => this.SetValue(RoundImage.BorderWidthProperty, value);
        }

        public bool HasShadow
        {
            get => (bool)this.GetValue(RoundImage.HasShadowProperty);
            set => this.SetValue(RoundImage.HasShadowProperty, value);
        }
    }
}
