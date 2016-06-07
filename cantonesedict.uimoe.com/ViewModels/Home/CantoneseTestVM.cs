using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cantonesedict.uimoe.com.ViewModels.Home
{
   public class CantoneseTestVM
    {
       public List<CantoneseTestItemVM> Items { get; set; }
    }

    public class CantoneseTestItemVM
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public List<CantoneseTestItemOptionVM> Options { get; set; }
    }

    public class CantoneseTestItemOptionVM
    {
        public string DisplayText { get; set; }

        public int InnerValue { get; set; }
    }
}
