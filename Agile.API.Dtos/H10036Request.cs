﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10036Request
    {
        public string rawurl { get; set; }

        public string useragent { get; set; }

        public int? userid { get; set; }

        public string ipaddress { get; set; }

        public int? domain { get; set; }
    }
}
