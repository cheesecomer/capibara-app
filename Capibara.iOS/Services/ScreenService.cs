using System;
using Capibara.Services;

using UIKit;

using Xamarin.Forms;

namespace Capibara.iOS.Services
{
    public class ScreenService : IScreenService
    {
        Size IScreenService.Size { get; } = 
            new Size(UIScreen.MainScreen.NativeBounds.Width, UIScreen.MainScreen.NativeBounds.Height); 
    }
}
