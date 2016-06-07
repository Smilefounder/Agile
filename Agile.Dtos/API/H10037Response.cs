using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10037Response : HBaseResponse
    {
        public List<H10037ResponseListItem> data { get; set; }
    }

    public class H10037ResponseListItem
    {
        public int id { get; set; }

        public string name { get; set; }
    }
}
