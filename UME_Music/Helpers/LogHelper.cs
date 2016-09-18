using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UME_Music.Helpers
{
    public class LogHelper
    {
        private static string _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        private static object _locker = new object();

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

        public static void WriteUseThread(object state)
        {
            lock (_locker)
            {
                var message = state as string;
                Write(message);
            }
        }

        public static void WriteAsync(string message)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(WriteUseThread), message);
        }
    }
}
