using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Models
{
    public class Can_score : T_base
    {
        [TableField]
        public int? Score { get; set; }

        [TableField]
        public int? Way { get; set; }

        [TableField]
        public int UserId { get; set; }
    }
}
