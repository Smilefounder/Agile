using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10044Response : HBaseResponse
    {
        public List<H10044ResponseListItem> data { get; set; }
    }

    public class H10044ResponseListItem
    {
        public int hasmenu { get; set; }

        public int domain { get; set; }

        public string name { get; set; }

        public string rawurl { get; set; }
    }
}
