using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10043Response : HBaseResponse
    {
        public PagedListDto<H10043ResponseListItem> data { get; set; }
    }

    public class H10043ResponseListItem
    {
        public string cantext { get; set; }

        public string canpronounce { get; set; }

        public string description { get; set; }
    }
}
