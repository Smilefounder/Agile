using Agile.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10073Response : HBaseResponse
    {
        public PagedListDto<H10073ResponseListItem> data { get; set; }
    }

    public class H10073ResponseListItem
    {
        public int id { get; set; }

        public DateTime? createdat { get; set; }

        public string name { get; set; }
    }
}
