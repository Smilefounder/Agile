using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10018Request
    {
        public int? skip { get; set; }

        public int? take { get; set; }

        public int? texttype { get; set; }
    }

    public enum H10018RequestTextTypeEnum
    {
        /// <summary>
        /// 字
        /// </summary>
        Character = 1,

        /// <summary>
        /// 词
        /// </summary>
        Term = 2,

        /// <summary>
        /// 句
        /// </summary>
        Sentence = 3
    }
}
