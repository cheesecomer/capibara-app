using System.Collections.Generic;
using Capibara.Domain.Models;

namespace Capibara.Domain.UseCases
{
    public interface IFetchRoomsUseCase : ISingleUseCase<ICollection<Room>> { }
}
