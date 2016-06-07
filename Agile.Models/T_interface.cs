using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Models
{
    public class T_interface : T_base
    {
        [TableField(MaxLength = 50)]
        public string Code { get; set; }

        [TableField(MaxLength = 50)]
        public string Name { get; set; }

        [TableField(MaxLength = 200)]
        public string Description { get; set; }
    }
}
