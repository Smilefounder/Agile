using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Threading;
using UME_Music.Models;

namespace UME_Music.Helpers
{
    public class CoreHelper
    {
        private static T_appConfig appConfig;
        /// <summary>
        /// 程序配置
        /// </summary>
        public static T_appConfig AppConfig
        {
            get
            {
                if (appConfig == null)
                {
                    appConfig = new T_appConfig
                    {
                        ThisStartUpAt = DateTime.Now
                    };
                }

                return appConfig;
            }

            set
            {
                appConfig = value;
            }
        }

        private static T_playerConfig playerConfig;
        /// <summary>
        /// 播放器配置
        /// </summary>
        public static T_playerConfig PlayerConfig
        {
            get
            {
                if (playerConfig == null)
                {
                    playerConfig = new T_playerConfig();
                }

                return playerConfig;
            }
            set
            {
                playerConfig = value;
            }
        }

        private static List<T_music> musiclist;
        /// <summary>
        /// 所有歌曲
        /// </summary>
        public static List<T_music> Musiclist
        {
            get
            {
                if (musiclist == null)
                {
                    musiclist = new List<T_music>();
                }

                return musiclist;
            }

            set
            {
                musiclist = value;
            }
        }

        private static T_music currentMusic;
        /// <summary>
        /// 当前正在播放的歌曲
        /// </summary>
        public static T_music CurrentMusic
        {
            get
            {
                if (currentMusic == null)
                {
                    currentMusic = new T_music();
                }

                return currentMusic;
            }

            set
            {
                currentMusic = value;
            }
        }

        /// <summary>
        /// 播放器状态
        /// </summary>
        public static int? PlayerState { get; set; }

        public delegate void PlayerStateChangedEventHandler();

        public delegate void PlayerMediaChangedEventHandler();

        /// <summary>
        /// 播放器状态变化时
        /// </summary>
        public static event PlayerStateChangedEventHandler PlayerStateChanged;

        /// <summary>
        /// 播放器媒体变化时
        /// </summary>
        public static event PlayerMediaChangedEventHandler PlayerMediaChanged;

        private static MediaPlayer player;
        /// <summary>
        /// 播放器
        /// </summary>
        public static MediaPlayer Player
        {
            get
            {
                if (player == null)
                {
                    player = new MediaPlayer();
                    player.MediaOpened += Player_MediaOpened;
                    player.MediaEnded += Player_MediaEnded;
                    player.MediaFailed += Player_MediaFailed;

                    MainTimer = new DispatcherTimer();
                    MainTimer.Interval = TimeSpan.FromSeconds(1);
                    MainTimer.Tick += MainTimer_Tick;
                    MainTimer.Start();

                }
                return player;
            }

            set
            {
                player = value;
            }
        }

        private static void MainTimer_Tick(object sender, EventArgs e)
        {
            if (PlayerState == (int)PlayerStateEnum.Playing)
            {
                if (PlayerStateChanged != null)
                {
                    PlayerStateChanged.Invoke();
                }
            }
        }

        private static DispatcherTimer MainTimer;

        /// <summary>
        /// 播放失败时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Player_MediaFailed(object sender, ExceptionEventArgs e)
        {
            LogHelper.Write("播放失败：" + Player.Source.AbsolutePath);
        }

        /// <summary>
        /// 播放结束时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Player_MediaEnded(object sender, EventArgs e)
        {
            LogHelper.Write("播放结束：" + Player.Source.AbsolutePath);
        }

        /// <summary>
        /// 播放开始时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Player_MediaOpened(object sender, EventArgs e)
        {
            LogHelper.Write("播放开始：" + Player.Source.LocalPath);
            var music = Musiclist.Where(w => w.FilePath == Player.Source.LocalPath).FirstOrDefault();
            if (music != null)
            {
                CurrentMusic = music;
            }

            if (PlayerMediaChanged != null)
            {
                PlayerMediaChanged.Invoke();
            }
        }

        /// <summary>
        /// 从文件添加音乐
        /// </summary>
        /// <param name="filepath"></param>
        public static int AddMusicFromFile(string filepath)
        {
            var fileinfo = new System.IO.FileInfo(filepath);
            var ext = fileinfo.Extension.ToUpper();
            if (ext != ".MP3" && ext != ".WAV" && ext != ".WMA")
            {
                throw new Exception("只支持.MP3,.WAV,.WMA格式的歌曲文件");
            }

            if (fileinfo.Length < 10 || fileinfo.Length > 100 * 1024 * 1024)
            {
                throw new Exception("文件必须大于10B，且小于100MB");
            }

            var count = Musiclist.Count(c => c.FilePath == filepath);
            if (count > 0)
            {
                throw new Exception("歌曲库中已存在此歌曲");
            }

            var id3info = ID3Helper.Read(filepath);
            if (id3info == null)
            {
                throw new Exception("读取歌曲ID3信息失败");
            }

            Musiclist.Add(new T_music
            {
                Album = id3info.GetFrame("TALB"),
                Artists = id3info.GetFrame("TPE1"),
                Title = id3info.GetFrame("TIT2"),
                FileExtension = fileinfo.Extension,
                FileLength = fileinfo.Length,
                FilePath = filepath
            });

            return 0;
        }

        /// <summary>
        /// 从目录添加音乐
        /// </summary>
        /// <param name="filepath"></param>
        public static void AddMusicFromDirectory(string directory)
        {
            //找到目录下的文件
            var files = System.IO.Directory.GetFiles(directory, "*.*", System.IO.SearchOption.TopDirectoryOnly);
            if (files != null && files.Any())
            {
                foreach (var f in files)
                {
                    try
                    {
                        CoreHelper.AddMusicFromFile(f);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Write(f + ": " + ex.Message);
                        continue;
                    }
                }
            }

            //找到目录下的目录
            var folders = System.IO.Directory.GetDirectories(directory);
            if (folders != null && folders.Any())
            {
                foreach (var f in folders)
                {
                    AddMusicFromDirectory(f);
                }
            }
        }

        /// <summary>
        /// 播放（从暂停转为播放）
        /// </summary>
        public static void Play()
        {
            Player.Play();
        }

        /// <summary>
        /// 播放（重新开始播放指定歌曲）
        /// </summary>
        /// <param name="filepath"></param>
        public static void Play(string filepath)
        {
            Player.Open(new Uri(filepath));
            Player.Play();
        }


        /// <summary>
        /// 播放（播放指定歌曲，可以预设播放进度）
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="position"></param>
        public static void Play(string filepath, double position)
        {
            Player.Open(new Uri(filepath));
            Player.Position = TimeSpan.FromSeconds(position);
            Player.Play();
        }
    }
}
