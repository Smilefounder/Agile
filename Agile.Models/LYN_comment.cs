using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Models
{
    public class LYN_comment : T_base
    {
        [TableField]
        public int? ArchiveId { get; set; }

        [TableField]
        public int? UserId { get; set; }

        [TableField(MaxLength = 200)]
        public string Content { get; set; }

        [TableField(MaxLength = 200)]
        public string IpAddress { get; set; }

        [TableField(MaxLength = 200)]
        public string UserAgent { get; set; }

        [TableField]
        public int? Status { get; set; }
    }
}
