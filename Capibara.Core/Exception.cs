using System;
namespace Capibara
{
    /// <summary>
    /// HTTP例外基底クラス
    /// </summary>
    public abstract class ExceptionBase : Exception { }

    /// <summary>
    /// 401 Unauthorized 例外
    /// </summary>
    public class UnauthorizedException : ExceptionBase { }

    /// <summary>
    /// 403 Forbidden 例外
    /// </summary>
    public class ForbiddenException : ExceptionBase { }

    /// <summary>
    /// 404 NotFound 例外
    /// </summary>
    public class NotFoundException : ExceptionBase { }

    /// <summary>
    /// 426 Upgrade Required 例外
    /// </summary>
    public class UpgradeRequiredException : ExceptionBase { }

    /// <summary>
    /// 503 ServiceUnavailable 例外
    /// </summary>
    public class ServiceUnavailableException : ExceptionBase { }
}
