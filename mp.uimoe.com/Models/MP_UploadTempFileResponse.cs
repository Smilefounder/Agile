using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mp.uimoe.com.Models
{
    public class MP_UploadTempFileResponse
    {
        public int errcode { get; set; }

        public string errmsg { get; set; }

        public string type { get; set; }

        public string media_id { get; set; }

        public long created_at { get; set; }
    }
}