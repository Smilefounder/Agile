using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10078Request : HPagedListRequest
    {
        public string username { get; set; }

        public string email { get; set; }

        public DateTime? createdat { get; set; }

        public int? domain { get; set; }

        public int? status { get; set; }
    }
}
