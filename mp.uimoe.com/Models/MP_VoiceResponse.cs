using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mp.uimoe.com.Models
{
    public class MP_VoiceResponse
    {
        public string ToUserName { get; set; }

        public string FromUserName { get; set; }

        public long CreateTime { get; set; }

        public string MsgType { get; set; }

        public string MediaId { get; set; }
    }
}