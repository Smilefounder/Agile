using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using UME_Music.Helpers;
using UME_Music.Models;

namespace UME_Music
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
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogHelper.Write(e.Exception.ToString());
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            LogHelper.Write("程序退出");

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var repositoryDirectory = System.IO.Path.Combine(new string[] { baseDirectory, "Repository" });
            if (!System.IO.Directory.Exists(repositoryDirectory))
            {
                System.IO.Directory.CreateDirectory(repositoryDirectory);
            }

            var musiclistfilepath = System.IO.Path.Combine(new string[] { repositoryDirectory, "Musiclist.ume" });
            SerializeHelper.ToXmlFile<List<T_music>>(musiclistfilepath, CoreHelper.Musiclist);
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var musiclistfilepath = System.IO.Path.Combine(new string[] { baseDirectory, "Repository", "Musiclist.ume" });
            if (System.IO.File.Exists(musiclistfilepath))
            {
                CoreHelper.Musiclist = SerializeHelper.ParseFromXmlFile<List<T_music>>(musiclistfilepath);
            }

            var appconfigfilepath = System.IO.Path.Combine(new string[] { baseDirectory, "Repository", "AppConfig.ume" });
            if (System.IO.File.Exists(appconfigfilepath))
            {
                CoreHelper.AppConfig = SerializeHelper.ParseFromXmlFile<T_appConfig>(appconfigfilepath);
            }

            var playerConfigFilePath = System.IO.Path.Combine(new string[] { baseDirectory, "Repository", "AppConfig.ume" });
            if (System.IO.File.Exists(playerConfigFilePath))
            {
                CoreHelper.PlayerConfig = SerializeHelper.ParseFromXmlFile<T_playerConfig>(playerConfigFilePath);
            }

            LogHelper.Write("程序启动");
        }
    }
}
