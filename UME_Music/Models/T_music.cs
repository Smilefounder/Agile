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

        public string Artists { get; set; }

        public string Album { get; set; }

        public double Duration { get; set; }

        public string FileExtension { get; set; }

        public long FileLength { get; set; }


        public string FilePath { get; set; }
    }
}
