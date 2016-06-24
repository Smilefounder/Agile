using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10060Request
    {
        public List<H10060RequestListItem> data { get; set; }
    }

    public class H10060RequestListItem
    {
        public string jokeid { get; set; }

        public string content { get; set; }

        public string pictureurl { get; set; }
    }
}
