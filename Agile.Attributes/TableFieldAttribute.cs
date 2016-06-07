using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Attributes
{
    /// <summary>
    /// 贴上该标签表示是数据库字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TableFieldAttribute : Attribute
    {
        /// <summary>
        /// 是否自增长
        /// </summary>
        public bool IsIdentity { get; set; }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 是否可空
        /// </summary>
        public bool IsNotNull { get; set; }

        /// <summary>
        /// 字段长度
        /// </summary>
        public int MaxLength { get; set; }
    }
}
