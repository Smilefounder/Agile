using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMEWebServer.Models
{
    public class Website
    {
        public string Name { get; set; }

        public string Directory { get; set; }

        public string IPAddress { get; set; }

        public int Port { get; set; }

        public string Domain { get; set; }
    }
}
