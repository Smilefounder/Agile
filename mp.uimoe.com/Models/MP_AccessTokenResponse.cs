using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mp.uimoe.com.Models
{
    public class MP_AccessTokenResponse
    {
        public int errcode { get; set; }

        public string errmsg { get; set; }

        public string access_token { get; set; }

        public long expires_in { get; set; }
    }
}