﻿using System.Collections.Generic;
using Capibara.Domain.Models;

namespace Capibara.Domain.UseCases
{
    public interface IFetchDirectMessageThreadUseCase : IObservableUseCase<ICollection<Message>> { }
}
