using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.HttpServer
{
    public class ViewResult : ActionResult
    {
        public string ViewPath { get; set; }
    }
}
