﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10084Request : HPagedListRequest
    {

        public int? sceneid { get; set; }
    }
}
