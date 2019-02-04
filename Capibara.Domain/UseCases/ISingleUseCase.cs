using System;
using System.Threading.Tasks;
namespace Capibara.Domain.UseCases
{
    public interface ISingleUseCase<TResult>
    {
        Task<TResult> Invoke();
    }

    public interface ISingleUseCase<TResult, TParam>
    {
        Task<TResult> Invoke(TParam param);
    }
}
