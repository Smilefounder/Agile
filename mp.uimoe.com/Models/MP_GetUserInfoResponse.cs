using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mp.uimoe.com.Models
{
    public class MP_GetUserInfoResponse
    {
        public int errcode { get; set; }

        public string errmsg { get; set; }

        public int subscribe { get; set; }

        public string openid { get; set; }

        public string nickname { get; set; }

        public int sex { get; set; }

        public string city { get; set; }

        public string country { get; set; }

        public string province { get; set; }

        public string language { get; set; }

        public string headimgurl { get; set; }

        public int subscribe_time { get; set; }

        public string unionid { get; set; }

        public string remark { get; set; }

        public int groupid { get; set; }

        public List<int> tagid_list { get; set; }
    }

    public enum MP_GetUserInfoResponseSexEnum
    {
        Unknown = 0,

        Male = 1,

        Female = 2
    }
}