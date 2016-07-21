using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UME_vocabulary.Helpers;

namespace UME_vocabulary
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Startup += App_Startup;
            Exit += App_Exit;
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            var path = System.IO.Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "dict2.txt" });
            using (var sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                var content = CoreHelper.GetRememberedStr();
                if (!string.IsNullOrEmpty(content))
                {
                    sw.Write(content);
                }
            }
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            var path1 = System.IO.Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "dict.txt" });
            if (System.IO.File.Exists(path1))
            {
                var lines1 = File.ReadAllLines(path1);
                CoreHelper.BuildVocabulary(lines1);
            }

            var path2 = System.IO.Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "dict2.txt" });
            if (System.IO.File.Exists(path2))
            {
                var lines2 = File.ReadAllLines(path2);
                CoreHelper.BuildRemembered(lines2);
            }
        }
    }
}
