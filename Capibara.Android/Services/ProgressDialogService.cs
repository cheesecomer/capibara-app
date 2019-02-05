using System.Reactive.Disposables;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Views;
using Android.Views.Animations;
using Capibara.Services;
using Reactive.Bindings.Extensions;

namespace Capibara.Droid.Services
{
    public class ProgressDialogService : IProgressDialogService
    {
        async Task IProgressDialogService.DisplayProgressAsync(Task task, string message)
        {
            var disposable = new CompositeDisposable();

            // Viewからインフレータを作成する
            LayoutInflater layoutInflater = LayoutInflater.From(MainActivity.Instance).AddTo(disposable);

            // 重ね合わせするViewの設定を行う
            var layoutParams = new WindowManagerLayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent,
                WindowManagerTypes.Application,
                WindowManagerFlags.WatchOutsideTouch,
                Format.Translucent)
                .AddTo(disposable);

            // WindowManagerを取得する
            //wm = (WindowManager)MainActivity.Instance.GetSystemService(Context.WindowService);
            var windowManager = MainActivity.Instance.WindowManager;

            // レイアウトファイルから重ね合わせするViewを作成する
            var view = layoutInflater.Inflate(Resource.Layout.overlay_progress, null).AddTo(disposable);

            // Viewを画面上に重ね合わせする
            windowManager.AddView(view, layoutParams);

            await task;

            var alphaAnimation = new AlphaAnimation(.7f, 0f) { Duration = 500 }.AddTo(disposable);
            view.StartAnimation(alphaAnimation);

            await Task.Delay(500);

            windowManager.RemoveView(view);

            disposable.Dispose();
        }
    }
}
