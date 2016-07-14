using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10011Request
    {
        public int? archiveid { get; set; }

        public int? isanonymous { get; set; }

        public string username { get; set; }

        public string content { get; set; }

        public string ipaddress { get; set; }

        public string useragent { get; set; }
    }
}
