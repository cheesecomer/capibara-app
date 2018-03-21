using System.Threading.Tasks;
namespace Capibara.Services
{
    /// <summary>
    /// <see cref="Task"/> の静的メソッドへのラッパーインターフェイス
    /// </summary>
    public interface ITaskService
    {
        Task Delay(int millisecondsDelay);
    }
}
