﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class HBaseResponse
    {
        public long time { get; set; }

        public int error { get; set; }

        public string message { get; set; }
    }
}
