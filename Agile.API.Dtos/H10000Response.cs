using Agile.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10000Response : HBaseResponse
    {
        public PagedListDto<H10000ResponseListItem> data { get; set; }
    }

    public class H10000ResponseListItem
    {
        public int Id { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }
    }
}
