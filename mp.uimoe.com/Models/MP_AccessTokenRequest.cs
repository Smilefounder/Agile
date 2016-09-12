using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mp.uimoe.com.Models
{
    public class MP_AccessTokenRequest
    {
        public string grant_type { get; set; }

        public string appid { get; set; }

        public string secret { get; set; }
    }
}