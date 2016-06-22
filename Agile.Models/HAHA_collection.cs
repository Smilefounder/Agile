using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Models
{
    public class HAHA_collection : T_base
    {
        [TableField(MaxLength = 50)]
        public string JokeId { get; set; }

        [TableField(MaxLength = 2000)]
        public string Content { get; set; }

        [TableField(MaxLength = 50)]
        public string PictureUrl { get; set; }
    }
}
