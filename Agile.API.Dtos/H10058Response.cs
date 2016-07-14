using Agile.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10058Response : HBaseResponse
    {
        public PagedListDto<H10058ResponseListItem> data { get; set; }
    }

    public class H10058ResponseListItem
    {
        public string jokeid { get; set; }

        public string content { get; set; }

        public string pictureurl { get; set; }
    }
}
