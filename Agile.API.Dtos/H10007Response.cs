using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10007Response : HBaseResponse
    {
        public List<H10007ResponseListItem> data { get; set; }
    }

    public class H10007ResponseListItem
    {
        public string name { get; set; }

        public int cnt { get; set; }
    }
}
