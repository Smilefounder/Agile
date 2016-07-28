using Agile.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10078Response : HBaseResponse
    {
        public PagedListDto<H10078ResponseListItem> data { get; set; }
    }

    public class H10078ResponseListItem
    {
        public string username { get; set; }

        public string email { get; set; }

        public DateTime? createdat { get; set; }

        public int id { get; set; }

        public int? domain { get; set; }

        public int? status { get; set; }
    }
}
