using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.HttpServer
{
    public class Response
    {
        public string HttpVersion { get; set; }

        public int Status { get; set; }

        public string Description
        {
            get
            {
                switch (Status)
                {
                    default:
                        return "";
                    case 200:
                        return "OK";
                }
            }
        }

        public Dictionary<string, string> Params { get; set; }

        public string Body { get; set; }
    }
}
