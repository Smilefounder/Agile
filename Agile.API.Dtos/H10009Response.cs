using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10009Response : HBaseResponse
    {
        public string token { get; set; }

        public int userid { get; set; }
    }
}
