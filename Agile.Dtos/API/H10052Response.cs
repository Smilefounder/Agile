using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10052Response : HBaseResponse
    {
        public PagedListDto<H10052ResponseListItem> data { get; set; }
    }

    public class H10052ResponseListItem
    {
        public string cantext { get; set; }

        public string canpronounce { get; set; }

        public string description { get; set; }
    }
}
