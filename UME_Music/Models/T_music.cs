using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UME_Music.Models
{
    public class T_music
    {
        public string Title { get; set; }

        public string TitleDisplay
        {
            get
            {
                if (string.IsNullOrEmpty(FilePath))
                {
                    return "柚萌音乐";
                }

                if (string.IsNullOrEmpty(Title))
                {
                    return System.IO.Path.GetFileNameWithoutExtension(FilePath);
                }

                return Title;
            }
        }

        public string Artists { get; set; }

        public string ArtistsDisplay
        {
            get
            {
                if (string.IsNullOrEmpty(Artists))
                {
                    return "未知歌手";
                }

                return Artists;
            }
        }

        public string Album { get; set; }

        public double Duration { get; set; }

        public string FileExtension { get; set; }

        public long FileLength { get; set; }


        public string FilePath { get; set; }
    }
}
