using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Models
{
    public class T_role : T_base
    {
        [TableField(MaxLength = 50)]
        public string Name { get; set; }
    }
}
