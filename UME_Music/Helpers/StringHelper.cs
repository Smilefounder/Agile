using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UME_Music.Helpers
{
    public class StringHelper
    {
        public static string GetTimeSpanStr(double seconds)
        {
            var ts = TimeSpan.FromSeconds(seconds);
            return GetTimeSpanStr(ts);
        }

        public static string GetTimeSpanStr(TimeSpan ts)
        {
            if (ts.TotalSeconds < 60)
            {
                return string.Format("{0}", ts.Seconds);
            }

            if (ts.TotalSeconds < 60 * 60)
            {
                return string.Format("{0}:{1}", ts.Minutes, ts.Seconds);
            }

            return string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
        }
    }
}
