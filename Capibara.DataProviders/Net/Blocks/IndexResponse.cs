using System.Collections.Generic;
namespace Capibara.Net.Blocks
{
    /// <summary>
    /// GET API_BASE/blocks のレスポンス
    /// </summary>
    public class IndexResponse
    {
        public IList<Models.Block> Blocks { get; set; } = new List<Models.Block>();
    }
}
