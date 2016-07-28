using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10079Request:HPagedListRequest
    {
        public string chntext { get; set; }

        public string cantext { get; set; }

        public string createdby { get; set; }

        public DateTime? createdat { get; set; }

        public int? status { get; set; }
    }
}
