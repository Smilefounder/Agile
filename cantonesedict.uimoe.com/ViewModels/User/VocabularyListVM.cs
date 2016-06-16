using Agile.Dtos;
using cantonesedict.uimoe.com.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cantonesedict.uimoe.com.ViewModels.User
{
    public class VocabularyListVM
    {
        public SkipTakeListDto<VocabularyListItemVM> data { get; set; }
    }
}