using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cantonesedict.uimoe.com.ViewModels.Home
{
    public class LyricResultVM
    {
        public string Title { get; set; }

        public string Artists { get; set; }

        public List<LyricResultListItemVM> Lines { get; set; }
    }

    public class LyricResultListItemVM
    {
        public string ChnText { get; set; }

        public string CanPronounce { get; set; }
    }
}
