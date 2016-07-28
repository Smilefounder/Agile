using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10080Request:HPagedListRequest
    {
        public string chntext { get; set; }

        public DateTime? createdat { get; set; }
    }
}
