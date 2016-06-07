using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mp.uimoe.com.Models
{
    public class MP_AuthorizeRequest
    {
        public string signature { get; set; }

        public string timestamp { get; set; }

        public string nonce { get; set; }

        public string echostr { get; set; }
    }
}
