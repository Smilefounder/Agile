using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10044Request
    {
        public int? domain { get; set; }

        public int? userid { get; set; }

        public string name { get; set; }

        public string rawurl { get; set; }
    }

    public enum H10044RequestDomainEnum
    {
        llyn23 = 0,

        uimoe = 1,

        api = 2,

        cantonesedict = 3,

        database = 4,

        mp = 5,

        nasa = 6,

        sharp = 7
    }
}
