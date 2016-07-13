using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Models
{
    public class UME_app : T_base
    {
        [TableField(MaxLength = 50)]
        public string Title { get; set; }

        [TableField(MaxLength = 200)]
        public string Description { get; set; }

        [TableField(MaxLength = 200)]
        public string Href { get; set; }

        [TableField]
        public int AppType { get; set; }
    }

    public enum AppTypeEnum
    {
        WebPage = 0,

        Weixin = 1,

        Desktop = 2,

        IOS = 3,

        Android = 4
    }
}
