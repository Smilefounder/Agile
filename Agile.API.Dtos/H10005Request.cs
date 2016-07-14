using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10005Request : HPagedListRequest
    {
        public string yearmonth { get; set; }
    }
}
