using Agile.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Models
{
    public class T_base
    {
        [TableField(IsIdentity = true, IsPrimaryKey = true, IsNotNull = true)]
        public int Id { get; set; }

        [TableField]
        public DateTime? CreatedAt { get; set; }
    }

    public enum DomainEnum
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
