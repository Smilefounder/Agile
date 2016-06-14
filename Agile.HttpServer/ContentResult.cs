using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.HttpServer
{
    public class ContentResult : ActionResult
    {
        public string Content { get; set; }
    }
}
