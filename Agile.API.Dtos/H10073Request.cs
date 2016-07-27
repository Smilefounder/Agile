using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10073Request : HPagedListRequest
    {
        public string name { get; set; }

        public DateTime? createdat { get; set; }
    }
}
