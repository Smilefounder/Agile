using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10014Response : HBaseResponse
    {
        public bool isallmatched { get; set; }

        public List<H10014ResponseListItem> data { get; set; }
    }

    public class H10014ResponseListItem
    {
        public string chntext { get; set; }

        public string cantext { get; set; }

        public string canpronounce { get; set; }

        public string canvoice { get; set; }

        public int rw { get; set; }
    }
}
