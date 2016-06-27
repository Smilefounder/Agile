using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cantonesedict.uimoe.com.ViewModels.User
{
    public class DoPlanVM
    {
        public string ChnText { get; set; }

        public string CanText { get; set; }

        public string CanPronounce { get; set; }

        public bool Finished { get; set; }

        public int VocabularyId { get; set; }
    }
}