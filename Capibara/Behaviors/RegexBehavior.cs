using System;
using System.Text.RegularExpressions;

using Xamarin.Forms;

namespace Capibara.Behaviors
{
    public class RegexBehavior : Behavior<Entry>
    {
        /// <summary>
        /// 正規表現
        /// </summary>
        private Regex _reg;

        /// <summary>
        /// 正規表現パターン
        /// </summary>
        public string RegexPattern { get { return _reg?.ToString(); } set { _reg = new Regex(value); } }

        /// <summary>
        /// Attached
        /// </summary>
        /// <param name="bindable"></param>
        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += TextChanged;
        }

        /// <summary>
        /// Detaching
        /// </summary>
        /// <param name="bindable"></param>
        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= TextChanged;
        }

        /// <summary>
        /// 入力テキスト変更時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue)) { return; }

            // 正規表現にマッチする場合 OK
            if ((_reg?.IsMatch(e.NewTextValue ?? string.Empty)).GetValueOrDefault()) return;

            // 正規表現にマッチしない場合、変更前の値に戻す
            // 以前の値が正規表現にマッチする場合は、以前の値
            // 以前の値が正規表現にマッチしない場合、string.Empty
            if ((_reg?.IsMatch(e.OldTextValue ?? string.Empty)).GetValueOrDefault())
            {
                ((Entry)sender).Text = e.OldTextValue;
                return;
            }

            ((Entry)sender).Text = string.Empty;
        }
    }
}
