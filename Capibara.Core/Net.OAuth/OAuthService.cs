using System.Threading.Tasks;
namespace Capibara.Net.OAuth
{
    public interface ITwitterOAuthService
    {
        Task<Session> AuthorizeAsync();

        Task<TokenPair> GetAccessTokenAsync(TokenPair requestTokenPair, string oauthVerifier);
    }
}
