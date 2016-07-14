using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10013Response : HBaseResponse
    {
        public List<H10013ResponseListItem> data { get; set; }
    }

    public class H10013ResponseListItem
    {
        public int id { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public string cover { get; set; }

        public string href { get; set; }
    }
}
