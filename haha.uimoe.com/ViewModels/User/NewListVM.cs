using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace haha.uimoe.com.ViewModels.User
{
    public class NewListVM
    {
        public List<NewListItemVM> data { get; set; }
    }

    public class NewListItemVM
    {
        public int badcount { get; set; }

        public int commentcount { get; set; }

        public string content { get; set; }

        public int goodcount { get; set; }

        public string jokeid { get; set; }

        public string pictureurl { get; set; }

        public string postedat { get; set; }

        public string postedby { get; set; }
    }

    public enum NewListTypeEnum
    {
        ByCreatedAt = 0,

        ByGoodCount = 1
    }
}