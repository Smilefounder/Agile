using Agile.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10080Response:HBaseResponse
    {
        public PagedListDto<H10080ResponseListItem> data { get; set; }
    }

    public class H10080ResponseListItem
    {
        public string chntext { get; set; }

        public DateTime? createdat { get; set; }

        public int id { get; set; }
    }
}
