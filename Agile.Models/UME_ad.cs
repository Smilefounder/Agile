using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Models
{
    public class UME_ad : T_base
    {
        [TableField(MaxLength = 50)]
        public string Url { get; set; }

        [TableField(MaxLength = 50)]
        public string Title { get; set; }

        [TableField(MaxLength = 50)]
        public string Cover { get; set; }

        [TableField]
        public decimal? Price { get; set; }

        [TableField]
        public decimal? Rate { get; set; }
    }
}
