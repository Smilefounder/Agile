using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mp.uimoe.com.Models
{
    public class MP_VoiceRequest : MP_NormalRequestBase
    {
        public string MediaId { get; set; }

        public string Format { get; set; }
    }
}
