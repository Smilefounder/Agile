using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10027Response : HBaseResponse
    {
        public H10027ResponseLine line { get; set; }

        public List<H10027ResponseStation> stations { get; set; }
    }

    public class H10027ResponseLine
    {
        public int id { get; set; }

        public string name { get; set; }

        public DateTime? startedat { get; set; }

        public DateTime? completedat { get; set; }

        public DateTime? openedat { get; set; }

        public int? status { get; set; }
    }

    public class H10027ResponseStation
    {
        public string name { get; set; }

        public int? status { get; set; }
    }
}
