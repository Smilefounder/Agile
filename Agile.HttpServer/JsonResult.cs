using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.HttpServer
{
    public class JsonResult : ActionResult
    {
        public object Data { get; set; }
    }
}
