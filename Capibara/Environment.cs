using System;
namespace Capibara
{
    public class EnvironmentLocal : IEnvironment
    {
        string IEnvironment.BaseUrl { get; } = "http://localhost:3000/";

        string IEnvironment.ApiBaseUrl { get; } = "http://localhost:3000/api";

        string IEnvironment.WebSocketUrl { get; } = "ws://localhost:3000/cable";

        string IEnvironment.OAuthBaseUrl { get; } = "http://localhost:3000/api/oauth/";

        string IEnvironment.PrivacyPolicyUrl { get; } = "https://capibara-staging.herokuapp.com/privacy_policy?from_app=1";

        string IEnvironment.TermsUrl { get; } = "https://capibara-staging.herokuapp.com/terms";

        int IEnvironment.WebSocketReceiveBufferSize { get; } = 1024;

        int IEnvironment.WebSocketSendBufferSize { get; } = 1024;
    }

    public class EnvironmentStaging : IEnvironment
    {
        string IEnvironment.BaseUrl { get; } = "https://capibara-staging.herokuapp.com/";

        string IEnvironment.ApiBaseUrl { get; } = "https://capibara-staging.herokuapp.com/api";

        string IEnvironment.WebSocketUrl { get; } = "ws://capibara-staging.herokuapp.com/cable";

        string IEnvironment.OAuthBaseUrl { get; } = "https://capibara-staging.herokuapp.com/api/oauth/";

        string IEnvironment.PrivacyPolicyUrl { get; } = "https://capibara-staging.herokuapp.com/privacy_policy?from_app=1";

        string IEnvironment.TermsUrl { get; } = "https://capibara-staging.herokuapp.com/terms";

        int IEnvironment.WebSocketReceiveBufferSize { get; } = 1024;

        int IEnvironment.WebSocketSendBufferSize { get; } = 1024;
    }

    public class EnvironmentProduction : IEnvironment
    {
        string IEnvironment.BaseUrl { get; } = "https://capibara-production.herokuapp.com/";

        string IEnvironment.ApiBaseUrl { get; } = "https://capibara-production.herokuapp.com/api";

        string IEnvironment.WebSocketUrl { get; } = "ws://capibara-production.herokuapp.com/cable";

        string IEnvironment.OAuthBaseUrl { get; } = "https://capibara-production.herokuapp.com/api/oauth/";

        string IEnvironment.PrivacyPolicyUrl { get; } = "https://capibara-production.herokuapp.com/privacy_policy?from_app=1";

        string IEnvironment.TermsUrl { get; } = "https://capibara-production.herokuapp.com/terms";

        int IEnvironment.WebSocketReceiveBufferSize { get; } = 1024;

        int IEnvironment.WebSocketSendBufferSize { get; } = 1024;
    }
}
