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
}
