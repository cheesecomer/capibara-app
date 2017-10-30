using System;
using System.Net.Http;

using System.Threading.Tasks;

namespace Capibara.Net
{
    public interface IRequest<TResponse> where TResponse : new()
    {
        Task<TResponse> Execute();
    }
}
