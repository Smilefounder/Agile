using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Helpers
{
    public class DateTimeHelper
    {
        /// <summary>
        /// 获得时间戳
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static double GetTimeStamp()
        {
            var dt = DateTime.Now;
            return GetTimeStamp(dt);
        }

        /// <summary>
        /// 获得时间戳
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static double GetTimeStamp(DateTime dt)
        {
            var timespan = dt - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            var timestamp = Math.Round(timespan.TotalMilliseconds);
            return timestamp;
        }
    }
}
