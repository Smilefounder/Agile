using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Models
{
    public class T_permission : T_base
    {
        [TableField(MaxLength = 50)]
        public string Name { get; set; }

        [TableField]
        public int? Domain { get; set; }

        [TableField(MaxLength = 50)]
        public string Area { get; set; }

        [TableField(MaxLength = 50)]
        public string Controller { get; set; }

        [TableField(MaxLength = 50)]
        public string Action { get; set; }
    }

    public enum PermissionDomainEnum
    {
        llyn23 = 0,

        uimoe = 1,

        api = 2,

        cantonesedict = 3,

        database = 4,

        mp = 5,

        nasa = 6,

        sharp = 7
    }
}
