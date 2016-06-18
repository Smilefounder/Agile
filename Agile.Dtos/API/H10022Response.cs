using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10022Response : HBaseResponse
    {
        public PagedListDto<H10022ResponseListItem> data { get; set; }
    }

    public class H10022ResponseListItem
    {
        public string chntext { get; set; }

        public string cantext { get; set; }

        public DateTime? createdat { get; set; }

        public int? status { get; set; }
    }

    public enum H10022ResponseFeedbackStatusEnum
    {
        Normal = 0,

        Pending = 1
    }
}
