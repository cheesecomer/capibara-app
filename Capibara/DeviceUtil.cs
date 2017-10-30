using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;

using Xamarin.Forms;

namespace Capibara
{
    public static class DeviceUtil
    {
        public static Task BeginInvokeOnMainThreadAsync(Action action)
        {
            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    action?.Invoke();
                    taskCompletionSource.SetResult(null);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            });

            return taskCompletionSource.Task;
        }
    }
}
