using System;
namespace Capibara.Domain.UseCases
{
    public interface ISingleUseCase<TResult>
    {
        IObservable<TResult> Invoke();
    }

    public interface ISingleUseCase<TResult, TParam>
    {
        IObservable<TResult> Invoke(TParam param);
    }
}
