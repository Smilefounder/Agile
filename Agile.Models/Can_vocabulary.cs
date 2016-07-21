using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Models
{
    public class Can_vocabulary : T_base
    {
        [TableField(MaxLength = 50)]
        public string ChnText { get; set; }

        [TableField(MaxLength = 50)]
        public string CanText { get; set; }

        [TableField(MaxLength = 50)]
        public string CanPronounce { get; set; }

        [TableField(MaxLength = 50)]
        public string CanVoice { get; set; }
    }
}
