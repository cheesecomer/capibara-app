using System;
namespace Capibara.Domain.UseCases
{
    public interface IObservableUseCase<TResult>
    {
        IObservable<TResult> Invoke();
    }

    public interface IObservableUseCase<TResult, TParam>
    {
        IObservable<TResult> Invoke(TParam param);
    }
}
