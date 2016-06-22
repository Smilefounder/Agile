using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10058Request : HPagedListRequest
    {
        public int? rtype { get; set; }
    }

    public enum H10058RequestTypeEnum
    {
        ByCreatedAt = 0,

        ByGoodCount = 1
    }
}
