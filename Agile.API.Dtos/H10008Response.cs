using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10008Response : HBaseResponse
    {
        public H10008ResponseListItem data { get; set; }
    }

    public class H10008ResponseListItem
    {
        public int id { get; set; }

        public string title { get; set; }

        public string content { get; set; }

        public DateTime? createdat { get; set; }
    }
}
