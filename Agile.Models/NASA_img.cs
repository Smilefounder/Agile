using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Models
{
    public class NASA_img : T_base
    {
        [TableField(MaxLength = 50)]
        public string Title { get; set; }

        [TableField(MaxLength = 200)]
        public string Description { get; set; }

        [TableField(MaxLength = 50)]
        public string FileName { get; set; }

        [TableField]
        public int? Status { get; set; }
    }
}
