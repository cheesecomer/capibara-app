using System.Threading.Tasks;

using AngleSharp;
using AngleSharp.Dom;

namespace Capibara.Services
{
    public interface IBrowsingContext
    {
        Task<IDocument> OpenAsync(string address);
    }

    public class BrowsingContext : IBrowsingContext
    {
        AngleSharp.IBrowsingContext browsingContext;

        public BrowsingContext(IConfiguration configuration)
        {
            this.browsingContext = AngleSharp.BrowsingContext.New(configuration);
        }

        Task<IDocument> IBrowsingContext.OpenAsync(string address) => this.browsingContext.OpenAsync(address);
    }

    public interface IBrowsingContextFactory
    {
        IBrowsingContext Create(IConfiguration configuration);
    }

    public class BrowsingContextFactory : IBrowsingContextFactory
    {
        IBrowsingContext IBrowsingContextFactory.Create(IConfiguration configuration) => new BrowsingContext(configuration);
    }
}
