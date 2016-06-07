using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10039Response : HBaseResponse
    {
        public List<H10039ResponseListItem> data { get; set; }
    }

    public class H10039ResponseListItem
    {
        public int score { get; set; }

        public int way { get; set; }

        public DateTime? createdat { get; set; }
    }
}
