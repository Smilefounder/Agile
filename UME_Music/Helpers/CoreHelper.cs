using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using UME_Music.Models;

namespace UME_Music.Helpers
{
    public class CoreHelper
    {
        private static List<T_music> musiclist;

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

        private static MediaPlayer player;

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
                }
                return player;
            }

            set
            {
                player = value;
            }
        }

        private static void Player_MediaFailed(object sender, ExceptionEventArgs e)
        {
            LogHelper.Write("播放失败：" + Player.Source.AbsolutePath);
        }

        private static void Player_MediaEnded(object sender, EventArgs e)
        {
            LogHelper.Write("播放结束：" + Player.Source.AbsolutePath);
        }

        private static void Player_MediaOpened(object sender, EventArgs e)
        {
            LogHelper.Write("播放开始：" + Player.Source.AbsolutePath);
        }

        public static void AddMusicFromFile(string filepath)
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
        }

        public static void Play()
        {
            Player.Play();
        }

        public static void Play(string filepath)
        {
            Player.Open(new Uri(filepath));
            Player.Play();
        }

        public static void Play(string filepath, double position)
        {
            Player.Open(new Uri(filepath));
            Player.Position = TimeSpan.FromSeconds(position);
            Player.Play();
        }
    }
}
