using System;
using Capibara.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(Screen))]
namespace Capibara.Droid
{
    public class Screen : IScreen
    {
        private Size? size;
        Size IScreen.Size
        {
            get
            {
                if (!size.HasValue)
                {
                    var statusBarHeight = 0.0d;
                    var totalHeight = 0.0d;
                    var contentHeight = 0.0d;
                    var contentwidth = 0.0d;

                    var resources = MainActivity.Instance.Resources;
                    var metrics = resources.DisplayMetrics;

                    var resourceId = 
                        resources.GetIdentifier("status_bar_height", "dimen", "android");
                    if (resourceId > 0)
                    {
                        statusBarHeight = resources.GetDimensionPixelSize(resourceId) / metrics.Density;
                        totalHeight = metrics.HeightPixels / metrics.Density;
                        contentHeight = totalHeight - statusBarHeight;

                        contentwidth = metrics.WidthPixels / metrics.Density;
                    }

                    this.size = new Size(contentwidth, contentHeight);
                }

                return size.Value;
            }
        }
    }
}
