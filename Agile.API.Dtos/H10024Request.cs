using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10024Request
    {
        public int? ttype { get; set; }

        public string username { get; set; }
    }

    public enum H10024RequestTypeEnum
    {
        Today = 0,

        ThisMonth = 1,

        ThisYear = 2,

        Total = 3
    }
}
