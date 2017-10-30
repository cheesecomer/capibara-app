using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Capibara.Net
{
    public interface IRestClient
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage);

        void ApplyRequestHeader(HttpRequestMessage requestMessage);

        AuthenticationHeaderValue GenerateAuthenticationHeader(string token);
    }
}
