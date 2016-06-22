using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Models
{
    public class T_menu : T_base
    {
        [TableField]
        public int? PermissionId { get; set; }
    }
}
