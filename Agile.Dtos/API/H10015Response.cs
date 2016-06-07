using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10015Response : HBaseResponse
    {
        public List<H10015ResponseListItem> data { get; set; }
    }

    public class H10015ResponseListItem
    {
        public string chntext { get; set; }

        public string cantext { get; set; }

        public string canpronounce { get; set; }

        public string canvoice { get; set; }
    }
}
