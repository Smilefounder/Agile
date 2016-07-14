using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10004Response : HBaseResponse
    {
        public List<H10004ResponseListItem> data { get; set; }
    }

    public class H10004ResponseListItem
    {
        public int id { get; set; }

        public string code { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public DateTime? createdat { get; set; }
    }
}
