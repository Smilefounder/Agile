using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.HttpServer
{
    public class Request
    {
        public string Method { get; set; }

        public string RawUrl { get; set; }

        public string HttpVersion { get; set; }

        public Dictionary<string,string> Params { get; set; }

        public string PostData { get; set; }
    }
}
