using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10024Response : HBaseResponse
    {
        public decimal cost { get; set; }

        public decimal income { get; set; }
    }
}
