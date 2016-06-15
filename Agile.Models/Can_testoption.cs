using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Models
{
    public class Can_testoption : T_base
    {
        [TableField]
        public int? TestItemId { get; set; }

        [TableField]
        public int? InnerValue { get; set; }

        [TableField(MaxLength = 50)]
        public string DisplayText { get; set; }
    }
}
