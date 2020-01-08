using System;
using System.Threading.Tasks;
using System.Reactive;

namespace Capibara.Domain.UseCases
{
    public interface ICompletableUseCase<TParam>
    {
        IObservable<Unit> Invoke(TParam param);
    }

    public interface ICompletableUseCase
    {
        IObservable<Unit> Invoke();
    }
}
