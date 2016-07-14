using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10041Request
    {
        public int? type { get; set; }

        public int? userid { get; set; }
    }

    public enum H10041RequestTypeEnum
    {
        Total = 0,

        Today = 1,

        ThisMonth = 2,

        ThisYear = 3
    }
}
