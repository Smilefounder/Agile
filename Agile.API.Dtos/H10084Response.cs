using Agile.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10084Response : HBaseResponse
    {
        public PagedListDto<H10084ResponseListItem> data { get; set; }
    }

    public class H10084ResponseListItem
    {
        public string chntext { get; set; }

        public int vocabularyid { get; set; }

        public int id { get; set; }
    }
}
