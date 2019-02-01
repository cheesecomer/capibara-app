using System;

using Android.Widget;

using Capibara.Services;

namespace Capibara.Droid.Services
{
    public class BalloonService : IBalloonService
    {
        void IBalloonService.DisplayBalloon(string message)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                var toast = Toast.MakeText(MainActivity.Instance, message, ToastLength.Short);
                toast.SetGravity(Android.Views.GravityFlags.Top | Android.Views.GravityFlags.Center, 0, 50);
                toast.Show();
            });
        }
    }
}
