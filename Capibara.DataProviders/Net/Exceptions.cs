using System;
using System.Collections.Generic;
using System.Net;

using Capibara.Models;

using Newtonsoft.Json;

namespace Capibara.Net
{
    /// <summary>
    /// HTTP例外基底クラス
    /// </summary>
    public abstract class HttpExceptionBase : Exception
    {
        public HttpExceptionBase(HttpStatusCode httpStatusCode, string message, Exception innerException)
            : base(message, innerException)
        {
            this.HttpStatusCode = httpStatusCode;
            this.Detail = JsonConvert.DeserializeObject<Error>(message);
        }

        public HttpExceptionBase(HttpStatusCode httpStatusCode, string message)
            : this(httpStatusCode, message, null)
        {
        }

        /// <summary>
        /// HTTPステータスコード
        /// </summary>
        /// <value>The http status code.</value>
        public HttpStatusCode HttpStatusCode { get; }

        /// <summary>
        /// エラー詳細
        /// </summary>
        /// <value>The detail.</value>
        public Error Detail { get; }
    }

    /// <summary>
    /// 404 NotFound 例外
    /// </summary>
    public class HttpNotFoundException : HttpExceptionBase
    {
        public HttpNotFoundException(HttpStatusCode httpStatusCode, string message, Exception innerException)
            : base(httpStatusCode, message, innerException)
        {
        }

        public HttpNotFoundException(HttpStatusCode httpStatusCode, string message)
            : this(httpStatusCode, message, null)
        {
        }
    }

    /// <summary>
    /// 401 Unauthorized 例外
    /// </summary>
    public class HttpUnauthorizedException : HttpExceptionBase
    {
        public HttpUnauthorizedException(HttpStatusCode httpStatusCode, string message, Exception innerException)
            : base(httpStatusCode, message, innerException)
        {
        }

        public HttpUnauthorizedException(HttpStatusCode httpStatusCode, string message)
            : this(httpStatusCode, message, null)
        {
        }
    }
}
