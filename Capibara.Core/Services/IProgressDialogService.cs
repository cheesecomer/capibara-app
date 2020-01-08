using System;

using System.Threading.Tasks;

namespace Capibara.Services
{
    public interface IProgressDialogService
    {
        IObservable<T> DisplayProgressAsync<T>(IObservable<T> task, string message = null);
        Task DisplayProgressAsync(Task task, string message = null);
        Task<T> DisplayProgressAsync<T>(Task<T> task, string message = null);
    }
}
