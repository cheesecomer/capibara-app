using System;
using System.Collections.Generic;
namespace Capibara.Net.Rooms
{
    /// <summary>
    /// GET API_BASE/rooms のレスポンス
    /// </summary>
    public class IndexResponse
    {
        public IList<Models.Room> Rooms { get; set; } = new List<Models.Room>();
    }
}
