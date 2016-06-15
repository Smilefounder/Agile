using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Models
{
    public class T_whitelist : T_base
    {
        [TableField]
        public int? UserId { get; set; }

        [TableField]
        public int? PermissionId { get; set; }
    }
}
