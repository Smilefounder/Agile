using Agile.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10005Response : HBaseResponse
    {
        public PagedListDto<H10005ResponseListItem> data { get; set; }
    }

    public class H10005ResponseListItem
    {
        public int id { get; set; }

        public string title { get; set; }

        public string content { get; set; }

        public DateTime? createdat { get; set; }
    }
}
