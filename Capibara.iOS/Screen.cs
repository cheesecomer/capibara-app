using System;
using Capibara.iOS;

using UIKit;

using Xamarin.Forms;

[assembly: Dependency(typeof(Screen))]
namespace Capibara.iOS
{
    public class Screen : IScreen
    {
        Size IScreen.Size { get; } = 
            new Size(UIScreen.MainScreen.NativeBounds.Width, UIScreen.MainScreen.NativeBounds.Height); 
    }
}
