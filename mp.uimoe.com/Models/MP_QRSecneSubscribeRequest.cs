using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mp.uimoe.com.Models
{
    public class MP_QRSecneSubscribeRequest : MP_SubscribeRequest
    {
        public string EventKey { get; set; }

        public string Ticket { get; set; }
    }
}
