using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10006Response : HBaseResponse
    {
        public List<H10006ResponseListItem> data { get; set; }
    }

    public class H10006ResponseListItem
    {
        public int id { get; set; }

        public string title { get; set; }
    }
}
