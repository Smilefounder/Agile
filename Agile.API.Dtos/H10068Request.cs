using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10068Request : HPagedListRequest
    {
        public string chntext { get; set; }

        public string cantext { get; set; }

        public string canpronounce { get; set; }

        public string canvoice { get; set; }

        public DateTime? createdat { get; set; }

        public int? textlength { get; set; }
    }
}
