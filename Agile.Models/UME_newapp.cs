using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Models
{
    public class UME_newapp : T_base
    {
        [TableField]
        public int? AppType { get; set; }

        [TableField(MaxLength = 200)]
        public string AppDesc { get; set; }

        [TableField(MaxLength = 50)]
        public string Email { get; set; }

        [TableField(MaxLength = 50)]
        public string PhoneNum { get; set; }
    }
}
