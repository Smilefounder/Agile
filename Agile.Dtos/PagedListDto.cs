using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos
{
    public class PagedListDto<T>
    {
        public long Page { get; set; }

        public long PageSize { get; set; }

        public long PageCount
        {
            get
            {
                if (PageSize == 0)
                {
                    return 0;
                }

                var count = Math.Ceiling(1.0 * RecordCount / PageSize);
                if (count > Int64.MaxValue)
                {
                    return Int64.MaxValue;
                }

                return Convert.ToInt64(count);
            }
        }


        public long RecordCount { get; set; }

        public List<T> RecordList { get; set; }
    }
}
