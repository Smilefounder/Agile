using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10019Response : HBaseResponse
    {
        public List<H10019ResponseListItem> items { get; set; }

        public List<H10019ResponseListItemOption> options { get; set; }
    }

    public class H10019ResponseListItem
    {
        public int id { get; set; }

        public string title { get; set; }
    }

    public class H10019ResponseListItemOption
    {
        public string displaytext { get; set; }

        public int? innervalue { get; set; }

        public int? testitemid { get; set; }
    }
}
