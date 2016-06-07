using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Helpers
{
    public class LogHelper
    {
        static LogHelper()
        {
            _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        private static string _baseDirectory = null;

        public static void Write(string message)
        {
            var dt = DateTime.Now;
            var logDirectory = Path.Combine(new string[] { _baseDirectory ?? "", "Log" });
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            var filename = dt.ToString("yyyy-MM-dd") + ".txt";
            var filepath = Path.Combine(new string[] { logDirectory, filename });
            using (var sw = new StreamWriter(filepath, true, Encoding.UTF8))
            {
                sw.WriteLine(dt.ToString("yyyy-MM-dd HH:mm:ss"));
                sw.WriteLine(message);
                sw.WriteLine();
            }
        }
    }
}
