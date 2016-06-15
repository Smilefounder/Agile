using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Models
{
    public class Can_testitem : T_base
    {
        [TableField(MaxLength = 50)]
        public string Title { get; set; }
    }
}
