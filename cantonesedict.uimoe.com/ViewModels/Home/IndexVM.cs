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

        public bool IsAllMatched
        {
            get
            {
                if (AllMatched != null)
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsNoneMatched
        {
            get
            {
                if (AllMatched == null && (OneMatches == null || OneMatches.Count == 0))
                {
                    return true;
                }

                return false;
            }
        }

        public IndexListItemVM AllMatched { get; set; }

        public List<IndexListItemVM> OneMatches { get; set; }
    }

    public class IndexListItemVM
    {
        public string ChnText { get; set; }

        public string CanText { get; set; }

        public string CanPronounce { get; set; }

        public string CanVoice { get; set; }
    }
}
