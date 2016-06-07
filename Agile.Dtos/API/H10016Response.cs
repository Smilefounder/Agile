using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10016Response : HBaseResponse
    {
        public List<H10016ResponseListItem> data { get; set; }
    }

    public class H10016ResponseListItem
    {
        public string chntext { get; set; }

        public string cantext { get; set; }

        public string canpronounce { get; set; }

        public string canvoice { get; set; }
    }
}
