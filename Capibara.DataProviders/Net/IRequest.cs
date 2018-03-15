using System;
using System.Net.Http;

using System.Threading.Tasks;

namespace Capibara.Net
{
    public interface IRequest
    {
        Task Execute();
    }

    public interface IRequest<TResponse> where TResponse : new()
    {
        Task<TResponse> Execute();
    }
}
