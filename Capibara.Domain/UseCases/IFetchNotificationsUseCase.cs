using System;
using System.Collections.Generic;

using Capibara.Domain.Models;

namespace Capibara.Domain.UseCases
{
    public interface IFetchNotificationsUseCase: IObservableUseCase<ICollection<Notification>> { }
}
