using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10076Response : HBaseResponse
    {
        public H10076ResponseListItem data { get; set; }
    }

    public class H10076ResponseListItem
    {
        public int id { get; set; }

        public DateTime? createdat { get; set; }

        public string name { get; set; }
    }
}
