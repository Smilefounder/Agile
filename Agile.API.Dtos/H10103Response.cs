using Agile.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10103Response : HBaseResponse
    {
        public PagedListDto<H10103ResponseListItem> data { get; set; }
    }

    public class H10103ResponseListItem
    {
        public DateTime? CreatedAt { get; set; }

        public string ImageUrl { get; set; }

        public string Postedby { get; set; }

        public string WeiboUrl { get; set; }

        public string Labels { get; set; }

        public int? Status { get; set; }

        public int? Ht { get; set; }

        public int? Wt { get; set; }
    }
}
