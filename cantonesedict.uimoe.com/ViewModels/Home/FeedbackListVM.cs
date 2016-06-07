﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cantonesedict.uimoe.com.ViewModels.Home
{
    public class FeedbackListVM
    {
        public List<FeedbackListItemVM> Data { get; set; }
    }

    public class FeedbackListItemVM
    {
        public string ChnText { get; set; }

        public string CanText { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? Status { get; set; }
    }
}
