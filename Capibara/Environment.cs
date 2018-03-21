using System;
namespace Capibara
{
    public class EnvironmentLocal : IEnvironment
    {
        string IEnvironment.BaseUrl { get; } = "http://localhost:3000/";

        string IEnvironment.ApiBaseUrl { get; } = "http://localhost:3000/api";

        string IEnvironment.WebSocketUrl { get; } = "ws://localhost:3000/cable";

        string IEnvironment.OAuthBaseUrl { get; } = "http://localhost:3000/api/oauth/";

        int IEnvironment.WebSocketReceiveBufferSize { get; } = 1024;

        int IEnvironment.WebSocketSendBufferSize { get; } = 1024;
    }

    public class EnvironmentStaging : IEnvironment
    {
        string IEnvironment.BaseUrl { get; } = "https://capibara-staging.herokuapp.com/";

        string IEnvironment.ApiBaseUrl { get; } = "https://capibara-staging.herokuapp.com/api";

        string IEnvironment.WebSocketUrl { get; } = "ws://capibara-staging.herokuapp.com/cable";

        string IEnvironment.OAuthBaseUrl { get; } = "https://capibara-staging.herokuapp.com/api/oauth/";

        int IEnvironment.WebSocketReceiveBufferSize { get; } = 1024;

        int IEnvironment.WebSocketSendBufferSize { get; } = 1024;
    }
}