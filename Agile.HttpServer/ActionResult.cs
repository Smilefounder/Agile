using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.HttpServer
{
    public class ActionResult
    {
        public int ContentType { get; set; }
    }

    public enum ContentTypeEnum
    {
        Text = 0,

        Html = 1,

        Json = 2
    }
}
