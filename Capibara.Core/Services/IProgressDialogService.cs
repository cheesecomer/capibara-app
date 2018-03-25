using System;

using System.Threading.Tasks;

namespace Capibara.Services
{
    public interface IProgressDialogService
    {
        Task DisplayProgressAsync(Task task, string message = null);
    }
}
