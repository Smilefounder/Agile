using Agile.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10068Response : HBaseResponse
    {
        public PagedListDto<H10068ResponseListItem> data { get; set; }
    }

    public class H10068ResponseListItem
    {
        public string ChnText { get; set; }

        public string CanText { get; set; }

        public string CanPronounce { get; set; }

        public string CanVoice { get; set; }

        public int Id { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
