using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cantonesedict.uimoe.com.ViewModels.Home
{
    public class VocabularyVM
    {
        public int Skip { get; set; }

        public int Take { get; set; }

        public List<VocabularyListItemVM> Data { get; set; }
    }

    public class VocabularyListItemVM
    {
        public string ChnText { get; set; }

        public string CanText { get; set; }

        public string CanPronounce { get; set; }

        public string CanVoice { get; set; }
    }
}
