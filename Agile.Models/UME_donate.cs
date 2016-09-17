using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Models
{
    public class UME_donate : T_base
    {
        [TableField(MaxLength = 50)]
        public string Name { get; set; }

        [TableField(MaxLength = 50)]
        public string NickName { get; set; }

        [TableField]
        public int? Source { get; set; }

        [TableField]
        public decimal? Money { get; set; }
    }
}
