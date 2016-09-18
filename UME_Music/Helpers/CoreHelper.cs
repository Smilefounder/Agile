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

        private static List<T_playRecord> _playRecords;
        /// <summary>
        /// 播放记录
        /// </summary>
        public static List<T_playRecord> PlayRecords
        {
            get
            {
                if (_playRecords == null)
                {
                    _playRecords = new List<T_playRecord>();
                }

                return _playRecords;
            }

            set
            {
                _playRecords = value;
            }
        }

        private static bool _isMusicListChanged = false;
        /// <summary>
        /// 歌曲库是否发生改变
        /// </summary>
        public static bool IsMusicListChanged
        {
            get
            {
                return _isMusicListChanged;
            }

            set
            {
                _isMusicListChanged = value;
            }
        }

        private static bool _isPlayRecordsChanged = false;
        /// <summary>
        /// 播放记录是否发生改变
        /// </summary>
        public static bool IsPlayRecordsChanged
        {
            get
            {
                return _isPlayRecordsChanged;
            }

            set
            {
                _isPlayRecordsChanged = value;
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
            if (PlayerState.HasValue)
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
            LogHelper.WriteAsync("播放失败：" + Player.Source.LocalPath);

            //清除播放器状态
            PlayerState = null;
        }

        /// <summary>
        /// 播放结束时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Player_MediaEnded(object sender, EventArgs e)
        {
            LogHelper.WriteAsync("播放结束：" + Player.Source.LocalPath);

            //清除播放器状态
            PlayerState = null;

            //添加播放记录
            PlayRecords.Add(new T_playRecord
            {
                CreatedAt = DateTime.Now,
                FilePath = Player.Source.LocalPath
            });

            //设置标识，退出程序时保存播放记录
            IsPlayRecordsChanged = true;

            //按播放模式进行下一次播放
            var mode = PlayerConfig.PlayMode.GetValueOrDefault();
            switch (mode)
            {
                case (int)PlayModeEnum.Order:
                    {
                        PlayByOrder();
                    }
                    break;
                case (int)PlayModeEnum.Random:
                    {
                        PlayByOrder();
                    }
                    break;
                case (int)PlayModeEnum.Recycle:
                    {
                        PlayByRecycle();
                    }
                    break;
                case (int)PlayModeEnum.Repeat:
                    {
                        PlayByRepeat();
                    }
                    break;
            }
        }

        /// <summary>
        /// 获取歌曲位置索引
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private static int GetMusicIndex(string filepath)
        {
            for (var i = 0; i < Musiclist.Count; i++)
            {
                var music = Musiclist[i];
                if (music.FilePath == filepath)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 顺序播放
        /// </summary>
        private static void PlayByOrder()
        {
            var idx = GetMusicIndex(CurrentMusic.FilePath);
            if (idx < 0)
            {
                return;
            }

            if (idx == Musiclist.Count - 1)
            {
                return;
            }

            idx += 1;

            var music = Musiclist[idx];
            Play(music.FilePath);
        }

        /// <summary>
        /// 随机播放
        /// </summary>
        private static void PlayByRandom()
        {
            var r = new Random();
            var idx = r.Next(0, Musiclist.Count);

            var music = Musiclist[idx];
            Play(music.FilePath);
        }

        /// <summary>
        /// 列表循环
        /// </summary>
        private static void PlayByRecycle()
        {
            var idx = GetMusicIndex(CurrentMusic.FilePath);
            if (idx < 0)
            {
                return;
            }

            if (idx == Musiclist.Count - 1)
            {
                idx = 0;
            }
            else
            {
                idx += 1;
            }

            var music = Musiclist[idx];
            Play(music.FilePath);
        }

        /// <summary>
        /// 单曲循环
        /// </summary>
        private static void PlayByRepeat()
        {
            Play(CurrentMusic.FilePath);
        }

        /// <summary>
        /// 播放上一曲
        /// </summary>
        public static void PlayThePrev()
        {
            if (Musiclist.Count <= 1)
            {
                return;
            }

            var idx = GetMusicIndex(CurrentMusic.FilePath);
            if (idx < 0)
            {
                return;
            }

            if (idx == 0)
            {
                idx = Musiclist.Count - 1;
            }
            else
            {
                idx -= 1;
            }

            var music = Musiclist[idx];
            Play(music.FilePath);
        }

        /// <summary>
        /// 播放下一首一曲
        /// </summary>
        public static void PlayTheNext()
        {
            if (Musiclist.Count <= 1)
            {
                return;
            }

            var idx = GetMusicIndex(CurrentMusic.FilePath);
            if (idx < 0)
            {
                return;
            }

            if (idx == Musiclist.Count-1)
            {
                idx = 0;
            }
            else
            {
                idx += 1;
            }

            var music = Musiclist[idx];
            Play(music.FilePath);
        }

        /// <summary>
        /// 播放或暂停
        /// </summary>
        public static void PlayPause()
        {
            if (!PlayerState.HasValue && Musiclist.Count>0)
            {
                var music = Musiclist[0];
                Play(music.FilePath);
                return;
            }

            if (PlayerState == (int)PlayerStateEnum.Playing)
            {
                Pause();
                return;
            }

            Play();
        }

        /// <summary>
        /// 设置播放模式
        /// </summary>
        public static void SetPlayMode()
        {
            var mode = PlayerConfig.PlayMode.GetValueOrDefault();
            if (mode < 3)
            {
                mode += 1;
            }
            else
            {
                mode = 0;
            }

            PlayerConfig.PlayMode = mode;
        }

        /// <summary>
        /// 播放开始时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Player_MediaOpened(object sender, EventArgs e)
        {
            LogHelper.WriteAsync("播放开始：" + Player.Source.LocalPath);
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

            IsMusicListChanged = true;
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
                        LogHelper.WriteAsync(f + ": " + ex.Message);
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
        /// 暂停（从播放转为暂停）
        /// </summary>
        public static void Pause()
        {
            if (Player.CanPause)
            {
                Player.Pause();
                PlayerState = (int)PlayerStateEnum.Paused;
            }
        }

        /// <summary>
        /// 播放（从暂停转为播放）
        /// </summary>
        public static void Play()
        {
            Player.Play();
            PlayerState = (int)PlayerStateEnum.Playing;
        }

        /// <summary>
        /// 播放（重新开始播放指定歌曲）
        /// </summary>
        /// <param name="filepath"></param>
        public static void Play(string filepath)
        {
            Player.Open(new Uri(filepath));
            Player.Play();
            PlayerState = (int)PlayerStateEnum.Playing;
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
            PlayerState = (int)PlayerStateEnum.Playing;
        }

        public static void RemoveMusic(string path)
        {
            var music = Musiclist.Where(w => w.FilePath == path).FirstOrDefault();
            if (music != null)
            {
                Musiclist.Remove(music);
                IsMusicListChanged = true;
            }
        }
    }
}
