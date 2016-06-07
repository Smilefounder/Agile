using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10009Request
    {
        public string username { get; set; }

        public string userpass { get; set; }

        public int? status { get; set; }

        public int? id { get; set; }
    }
}
