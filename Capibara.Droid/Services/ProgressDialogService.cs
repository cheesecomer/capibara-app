using System.Threading.Tasks;

using Android.Graphics;
using Android.Views;

using Capibara.Services;

using Com.Airbnb.Lottie;

namespace Capibara.Droid.Services
{
    public class ProgressDialogService : IProgressDialogService
    {
        async Task IProgressDialogService.DisplayProgressAsync(Task task, string message)
        {
            // Viewからインフレータを作成する
            LayoutInflater layoutInflater = LayoutInflater.From(MainActivity.Instance);

            // 重ね合わせするViewの設定を行う
            var layoutParams = new WindowManagerLayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent,
                WindowManagerTypes.Application,
                WindowManagerFlags.WatchOutsideTouch,
                Format.Translucent);

            // WindowManagerを取得する
            //wm = (WindowManager)MainActivity.Instance.GetSystemService(Context.WindowService);
            var windowManager = MainActivity.Instance.WindowManager;

            // レイアウトファイルから重ね合わせするViewを作成する
            var view = layoutInflater.Inflate(Resource.Layout.overlay_progress, null);

            var animationView = view.FindViewById<LottieAnimationView>(Resource.Id.animation_view);
            //animationView

            // Viewを画面上に重ね合わせする
            windowManager.AddView(view, layoutParams);

            await task;

            view.StartAnimation(new Android.Views.Animations.AlphaAnimation(.7f, 0f) { Duration = 500 });

            windowManager.RemoveView(view);
        }
    }
}
