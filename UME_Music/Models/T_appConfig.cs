using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UME_Music.Models
{
    public class T_appConfig
    {
        /// <summary>
        /// 上次启动时间
        /// </summary>
        public DateTime? LastStartUpAt { get; set; }

        /// <summary>
        /// 本次启动时间
        /// </summary>
        public DateTime? ThisStartUpAt { get; set; }
    }
}
