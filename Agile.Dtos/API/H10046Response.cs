﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Dtos.API
{
    public class H10046Response : HBaseResponse
    {
        public List<H10018ResponseListItem> data { get; set; }
    }
}
