using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UME_Music.ViewModels
{
    public class PlayRecordVM
    {
        public string Title { get; set; }

        public string Artists { get; set; }

        public string FilePath { get; set; }

        public DateTime PlayedAt { get; set; }
    }
}
