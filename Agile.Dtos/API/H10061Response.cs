using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10061Response : HBaseResponse
    {
        public PagedListDto<H10061ResponseListItem> data { get; set; }
    }

    public class H10061ResponseListItem
    {
        public string name { get; set; }

        public int sceneid { get; set; }

        public int total { get; set; }

        public int finished { get; set; }

        public int percent
        {
            get
            {
                var p = 0;
                if (total > 0)
                {
                    p = Convert.ToInt32(Math.Ceiling(100.0 * finished / total));
                }

                return p;
            }
        }
    }
}
