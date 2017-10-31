using System;

using System.Threading.Tasks;

namespace Capibara.Services
{
    public interface IProgressDialogService
    {
        Task DisplayAlertAsync(Task task, string message = null);
    }
}
