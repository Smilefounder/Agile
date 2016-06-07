using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10028Response : HBaseResponse
    {
        public List<H10028ResponseListItem> data { get; set; }
    }

    public class H10028ResponseListItem
    {
        public string stationname { get; set; }

        public string linename { get; set; }

        public int? status { get; set; }
    }
}
