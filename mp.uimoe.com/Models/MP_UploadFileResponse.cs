using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mp.uimoe.com.Models
{
    public class MP_UploadFileResponse
    {
        public int errcode { get; set; }

        public string errmsg { get; set; }

        public string media_id { get; set; }

        public string url { get; set; }
    }
}