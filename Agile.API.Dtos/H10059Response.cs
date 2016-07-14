using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10059Response : HBaseResponse
    {
        public List<H10059ResponseListItem> data { get; set; }
    }

    public class H10059ResponseListItem
    {
        public string content { get; set; }
    }
}
