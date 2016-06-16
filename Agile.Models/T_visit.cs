using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Models
{
    public class T_visit : T_base
    {
        [TableField]
        public int? Domain { get; set; }

        [TableField(MaxLength = 200)]
        public string RawUrl { get; set; }

        [TableField(MaxLength = 200)]
        public string UserAgent { get; set; }

        [TableField]
        public int? UserId { get; set; }

        [TableField(MaxLength = 50)]
        public string IPAddress { get; set; }
    }
}
