using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10018Response : HBaseResponse
    {
        public SkipTakeListDto<H10018ResponseListItem> data { get; set; }
    }

    public class H10018ResponseListItem
    {
        public string chntext { get; set; }

        public string cantext { get; set; }

        public string canpronounce { get; set; }

        public string canvoice { get; set; }
    }
}
