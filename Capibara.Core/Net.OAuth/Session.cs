using System;
namespace Capibara.Net.OAuth
{
    public class Session
    {
        public TokenPair RequestToken { get; set; }

        public Uri AuthorizeUri { get; set; }
    }
}
