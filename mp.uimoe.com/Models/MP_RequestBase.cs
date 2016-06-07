using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace mp.uimoe.com.Models
{
    [Serializable]
    [XmlRoot("xml")]
    public class MP_RequestBase
    {
        public string ToUserName { get;set; }

        public string FromUserName { get; set; }

        public int CreateTime { get; set; }

        public string MsgType { get; set; }
    }
}
