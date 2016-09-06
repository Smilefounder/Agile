using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Models
{
    public class Fod_collection : T_base
    {
        [TableField(MaxLength = 50)]
        public string ImageUrl { get; set; }

        [TableField(MaxLength = 50)]
        public string Postedby { get; set; }

        [TableField(MaxLength = 50)]
        public string WeiboUrl { get; set; }

        [TableField(MaxLength = 200)]
        public string Labels { get; set; }

        [TableField]
        public int? Status { get; set; }

        [TableField]
        public int? Ht { get; set; }

        [TableField]
        public int? Wt { get; set; }
    }
}
