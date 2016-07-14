using Agile.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10045Response : HBaseResponse
    {
        public PagedListDto<H10045ResponseListItem> data { get; set; }
    }

    public class H10045ResponseListItem
    {
        public string ipaddress { get; set; }

        public string rawurl { get; set; }

        public string useragent { get; set; }

        public int? userid { get; set; }

        public DateTime? createdat { get; set; }
    }
}
