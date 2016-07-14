using Agile.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10033Response : HBaseResponse
    {
        public PagedListDto<H10033ResponseListItem> data { get; set; }
    }

    public class H10033ResponseListItem
    {
        public string title { get; set; }

        public string filename { get; set; }

        public DateTime? createdat { get; set; }
    }
}
