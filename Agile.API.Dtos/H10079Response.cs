using Agile.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10079Response : HBaseResponse
    {
        public PagedListDto<H10079ResponseListItem> data { get; set; }
    }

    public class H10079ResponseListItem
    {
        public string chntext { get; set; }

        public string cantext { get; set; }

        public string createdby { get; set; }

        public DateTime? createdat { get; set; }

        public int? status { get; set; }
    }
}
