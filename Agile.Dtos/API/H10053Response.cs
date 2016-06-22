using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10053Response:HBaseResponse
    {
        public PagedListDto<H10053ResponseListItem> data { get; set; }
    }

    public class H10053ResponseListItem
    {
        public string chntext { get; set; }
    }
}
