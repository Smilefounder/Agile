using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos
{
    public class SkipTakeListDto<T>
    {
        public int Skip { get; set; }

        public int Take { get; set; }

        public List<T> RecordList { get; set; }
    }
}
