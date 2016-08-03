using System;
using System.Collections.Generic;
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

            var appconfigfilepath = System.IO.Path.Combine(new string[] { baseDirectory, "Repository", "AppConfig.ume" });
            SerializeHelper.ToXmlFile<T_appConfig>(appconfigfilepath, CoreHelper.AppConfig);

            var playerConfigFilePath = System.IO.Path.Combine(new string[] { baseDirectory, "Repository", "PlayerConfig.ume" });
            SerializeHelper.ToXmlFile<T_playerConfig>(playerConfigFilePath, CoreHelper.PlayerConfig);

            if (CoreHelper.IsMusicListChanged)
            {
                var musiclistfilepath = System.IO.Path.Combine(new string[] { repositoryDirectory, "Musiclist.ume" });
                if (CoreHelper.Musiclist.Count == 0 && System.IO.File.Exists(musiclistfilepath))
                {
                    System.IO.File.Delete(musiclistfilepath);
                }
                else
                {
                    SerializeHelper.ToXmlFile<List<T_music>>(musiclistfilepath, CoreHelper.Musiclist);
                }
            }

            if (CoreHelper.IsPlayRecordsChanged)
            {
                var playRecordsFilePath = System.IO.Path.Combine(new string[] { repositoryDirectory, "PlayRecords.ume" });
                if (CoreHelper.PlayRecords.Count == 0 && System.IO.File.Exists(playRecordsFilePath))
                {
                    System.IO.File.Delete(playRecordsFilePath);
                }
                else
                {
                    SerializeHelper.ToXmlFile<List<T_playRecord>>(playRecordsFilePath, CoreHelper.PlayRecords);
                }
            }
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            LogHelper.Write("程序启动");

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

            var playerConfigFilePath = System.IO.Path.Combine(new string[] { baseDirectory, "Repository", "PlayerConfig.ume" });
            if (System.IO.File.Exists(playerConfigFilePath))
            {
                CoreHelper.PlayerConfig = SerializeHelper.ParseFromXmlFile<T_playerConfig>(playerConfigFilePath);
            }

            var playerRecordsFilePath = System.IO.Path.Combine(new string[] { baseDirectory, "Repository", "PlayRecords.ume" });
            if (System.IO.File.Exists(playerRecordsFilePath))
            {
                CoreHelper.PlayRecords = SerializeHelper.ParseFromXmlFile<List<T_playRecord>>(playerRecordsFilePath);
            }
        }
    }
}
