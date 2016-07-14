using Agile.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10031Response : HBaseResponse
    {
        public PagedListDto<H10031ResponseListItem> data { get; set; }
    }

    public class H10031ResponseListItem
    {
        public DateTime? createdat { get; set; }

        public string content { get; set; }
    }
}
