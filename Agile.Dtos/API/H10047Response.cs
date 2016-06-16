using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10047Response : HBaseResponse
    {
        public PagedListDto<H10047ResponseListItem> data { get; set; }
    }

    public class H10047ResponseListItem
    {
        public string username { get; set; }

        public int? status { get; set; }
    }

    public enum H10047UserStatusEnum
    {
        Normal = 0,

        Pending = 1,

        Forbidden = 2
    }
}
