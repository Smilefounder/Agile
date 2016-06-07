using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10038Response : HBaseResponse
    {
        public PagedListDto<H10038ResponseListItem> data { get; set; }
    }

    public class H10038ResponseListItem
    {
        public string chntext { get; set; }

        public string cantext { get; set; }

        public string canpronounce { get; set; }
    }
}
