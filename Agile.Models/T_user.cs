using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Models
{
    public class T_user : T_base
    {
        [TableField(MaxLength = 50)]
        public string UserName { get; set; }

        [TableField(MaxLength = 50)]
        public string UserPass { get; set; }

        [TableField(MaxLength = 50)]
        public string Email { get; set; }

        [TableField]
        public int? Status { get; set; }
    }
}
