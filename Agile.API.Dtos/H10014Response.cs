using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10014Response : HBaseResponse
    {
        public List<H10014ResponseGroupItem> groups { get; set; }

        public List<string> noresult { get; set; }
    }

    public class H10014ResponseGroupItem
    {
        public int rw { get; set; }

        public string chntext { get; set; }

        public List<H10014ResponseListItem> items { get; set; }
    }

    public class H10014ResponseListItem
    {
        public string cantext { get; set; }

        public string canpronounce { get; set; }

        public string canvoice { get; set; }
    }

    public class H10014ResponseOrderItem: H10014ResponseListItem
    {
        public int rw { get; set; }

        public string chntext { get; set; }
    }
}
