using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cantonesedict.uimoe.com.ViewModels.Home
{
    public class IndexVM
    {
        public string Input { get; set; }

        public List<IndexGroupItemVM> Data { get; set; }
    }

    public class IndexGroupItemVM
    {
        public int RW { get; set; }

        public string ChnText { get; set; }

        public List<IndexListItemVM> Items { get; set; }
    }

    public class IndexListItemVM
    {
        public string CanText { get; set; }

        public string CanPronounce { get; set; }

        public string CanVoice { get; set; }

        public string CanVoiceUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(CanPronounce))
                {
                    return string.Format("http://yueyv.cn/voice/{0}.mp3", CanPronounce.Trim());
                }

                return CanVoice;
            }
        }
    }
}
