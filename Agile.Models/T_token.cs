using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Models
{
    public class T_token : T_base
    {
        [TableField(MaxLength = 50)]
        public string Token { get; set; }

        [TableField]
        public int? UserId { get; set; }
    }
}
