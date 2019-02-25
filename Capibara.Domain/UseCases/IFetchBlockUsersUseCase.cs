using System.Collections.Generic;
using Capibara.Domain.Models;

namespace Capibara.Domain.UseCases
{
    public interface IFetchBlockUsersUseCase : ISingleUseCase<ICollection<Block>> { }
}
