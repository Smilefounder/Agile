using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10012Response : HBaseResponse
    {
        public List<H10012ResponseListItem> data { get; set; }
    }

    public class H10012ResponseListItem
    {
        public string username { get; set; }

        public string content { get; set; }

        public DateTime? createdat { get; set; }
    }
}
