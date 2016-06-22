using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10058Response : HBaseResponse
    {
        public PagedListDto<H10058ResponseListItem> data { get; set; }
    }

    public class H10058ResponseListItem
    {
        public string jokeid { get; set; }

        public string postedat { get; set; }

        public string postedby { get; set; }

        public string content { get; set; }

        public string pictureurl { get; set; }

        public int goodcount { get; set; }

        public int badcount { get; set; }

        public int commentcount { get; set; }
    }
}
