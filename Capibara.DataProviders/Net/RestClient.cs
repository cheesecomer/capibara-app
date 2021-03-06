﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Capibara.Services;

namespace Capibara.Net
{
    public class RestClient : IRestClient, IDisposable
    {
        private HttpClient httpClient = new HttpClient();

        Task<HttpResponseMessage> IRestClient.SendAsync(HttpRequestMessage requestMessage)
             => this.httpClient.SendAsync(requestMessage);

        void IRestClient.ApplyRequestHeader(HttpRequestMessage requestMessage, IApplicationService applicationService)
        {
            requestMessage.Headers.Add("X-Platform", applicationService.Platform);
            requestMessage.Headers.Add("X-ApplicationVersion", applicationService.AppVersion);
            requestMessage.Headers.Add("X-DeviceId", applicationService.UUID);
        }

        AuthenticationHeaderValue IRestClient.GenerateAuthenticationHeader(string token)
            => new AuthenticationHeaderValue("Token", token);

        void IDisposable.Dispose()
        {
            this.httpClient?.Dispose();
        }
    }
}
