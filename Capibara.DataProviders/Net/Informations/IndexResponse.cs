using System;
using System.Collections;
using System.Collections.Generic;

namespace Capibara.Net.Informations
{
    public class IndexResponse
    {
        public IList<Models.Information> Informations { get; set; } = new List<Models.Information>();
    }
}
