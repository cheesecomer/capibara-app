using System.Collections.Generic;
namespace Capibara.Net.Follows
{
    /// <summary>
    /// GET API_BASE/follows のレスポンス
    /// </summary>
    public class IndexResponse
    {
        public IList<Models.Follow> Follows { get; set; } = new List<Models.Follow>();
    }
}
