using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mp.uimoe.com.Models
{
    public class MP_ScanRequest : MP_EventRequestBase
    {
        public string EventKey { get; set; }

        public string Ticket { get; set; }
    }
}
