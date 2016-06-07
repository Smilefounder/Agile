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
    public class MP_TextRequest : MP_NormalRequestBase
    {
        public string Content { get; set; }
    }
}
