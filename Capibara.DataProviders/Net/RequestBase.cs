using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

using Capibara.Services;

using Newtonsoft.Json;
using Unity;
using Unity.Attributes;

namespace Capibara.Net
{
    public abstract class RequestBaseCore
    {
        /// <summary>
        /// HTTP メソッドを取得します。
        /// </summary>
        /// <value>The method.</value>
        [JsonIgnore]
        public abstract HttpMethod Method { get; }

        /// <summary>
        /// パスを取得します。リクエスト時は Option#BaseUrl と結合されます。
        /// </summary>
        /// <value>The end point.</value>
        [JsonIgnore]
        public abstract string[] Paths { get; }

        /// <summary>
        /// Authentication Header を付与する必要があるか
        /// </summary>
        /// <value><c>true</c> if need authentication; otherwise, <c>false</c>.</value>
        [JsonIgnore]
        public virtual bool NeedAuthentication { get; } = false;

        /// <summary>
        /// DIコンテナ
        /// </summary>
        /// <value>The container.</value>
        [JsonIgnore]
        [Dependency]
        public IUnityContainer Container { get; set; }

        /// <summary>
        /// Gets or sets the environment.
        /// </summary>
        /// <value>The environment.</value>
        [JsonIgnore]
        [Dependency]
        public IEnvironment Environment { get; set; }

        /// <summary>
        /// Gets or sets the rest client.
        /// </summary>
        /// <value>The rest client.</value>
        [JsonIgnore]
        [Dependency]
        public IRestClient RestClient { get; set; }

        /// <summary>
        /// セキュア分離ストレージ
        /// </summary>
        /// <value>The secure isolated storage.</value>
        [JsonIgnore]
        [Dependency]
        public IIsolatedStorage IsolatedStorage { get; set; }

        [JsonIgnore]
        [Dependency]
        public ICapibaraApplication Application { get; set; }

        [JsonIgnore]
        [Dependency]
        public IApplicationService ApplicationService { get; set; }

        [JsonIgnore]
        public virtual string StringContent => string.Empty;

        [JsonIgnore]
        public virtual string ContentType => string.Empty;

        internal async Task<HttpResponseMessage> ExecuteInternal()
        {
            if (!this.Application.HasPlatformInitializer)
            {
                return null;
            }

            var path = Path.Combine(this.Paths);
            var url = Path.Combine(this.Environment.ApiBaseUrl, path);

            var requestMessage = new HttpRequestMessage(this.Method, url);

            this.RestClient.BuildUp(this.Container).ApplyRequestHeader(requestMessage, this.ApplicationService);

            if (this.NeedAuthentication && this.IsolatedStorage.AccessToken.IsPresent())
            {
                requestMessage.Headers.Authorization
                      = this.RestClient.GenerateAuthenticationHeader(this.IsolatedStorage.AccessToken);
            }

            if (new HttpMethod[] { HttpMethod.Post, HttpMethod.Put }.Any(x => x == this.Method))
            {
                requestMessage.Content = new StringContent(this.StringContent);
                requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(this.ContentType);
            }

            var responseMessage = await this.RestClient.SendAsync(requestMessage);
            requestMessage.Dispose();

            if (responseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                throw new HttpNotFoundException(responseMessage.StatusCode, await responseMessage.Content.ReadAsStringAsync());
            }
            else if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                this.IsolatedStorage.UserId = 0;
                this.IsolatedStorage.UserNickname = string.Empty;
                this.IsolatedStorage.AccessToken = string.Empty;
                this.IsolatedStorage.Save();

                throw new HttpUnauthorizedException(responseMessage.StatusCode, await responseMessage.Content.ReadAsStringAsync());
            }
            else if (responseMessage.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new HttpForbiddenException(responseMessage.StatusCode, await responseMessage.Content.ReadAsStringAsync());
            }
            else if (responseMessage.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                throw new HttpServiceUnavailableException(responseMessage.StatusCode, await responseMessage.Content.ReadAsStringAsync());
            }
            else if (responseMessage.StatusCode == HttpStatusCode.UpgradeRequired)
            {
                throw new HttpUpgradeRequiredException(responseMessage.StatusCode, await responseMessage.Content.ReadAsStringAsync());
            }

            return responseMessage;
        }
    }

    public abstract class RequestBase : RequestBaseCore, IRequest
    {
        /// <summary>
        /// Execute this instance.
        /// </summary>
        /// <returns>The execute.</returns>
        public virtual async Task Execute()
        {
            (await this.ExecuteInternal())?.Dispose();
        }
    }

    public abstract class RequestBase<TResponse> : RequestBaseCore, IRequest<TResponse> where TResponse : class, new()
    {
        /// <summary>
        /// Execute this instance.
        /// </summary>
        /// <returns>The execute.</returns>
        public virtual async Task<TResponse> Execute()
        {
            var responseMessage = await this.ExecuteInternal();
            if (responseMessage == null)
            {
                return null;
            }

            try
            {
                using (var stream = await responseMessage.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                {
                    var response = reader.ReadToEnd();
                    if (response.IsNullOrEmpty())
                    {
                        return new TResponse().BuildUp(this.Container);
                    }

                    return JsonConvert.DeserializeObject<TResponse>(response).BuildUp(this.Container);
                }
            }
            finally
            {
                responseMessage.Dispose();
            }
        }
    }
}
