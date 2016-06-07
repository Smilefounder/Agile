using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mp.uimoe.com.Models
{
    public class MP_LocationEventRequest : MP_EventRequestBase
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Precision { get; set; }
    }
}
