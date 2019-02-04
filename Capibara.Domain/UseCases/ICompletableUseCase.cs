using System;
using System.Threading.Tasks;

namespace Capibara.Domain.UseCases
{
    public interface ICompletableUseCase<TParam>
    {
        Task Invoke(TParam param);
    }

    public interface ICompletableUseCase
    {
        Task Invoke();
    }
}
