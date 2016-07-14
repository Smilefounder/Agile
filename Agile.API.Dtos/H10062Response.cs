using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10062Response : HBaseResponse
    {
        public H10062ResponseListItem data { get; set; }
    }

    public class H10062ResponseListItem
    {
        public string chntext { get; set; }

        public string cantext { get; set; }

        public string canpronounce { get; set; }

        public int finished { get; set; }

        public int id { get; set; }
    }
}
