using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cantonesedict.uimoe.com.ViewModels.Reimu
{
    public class StatisticsVM
    {
        public string now { get; set; }

        public int wordcount { get; set; }

        public int termcount { get; set; }

        public int sentencecount { get; set; }

        public int querycount { get; set; }

        public int todayquerycount { get; set; }

        public int noresultcount { get; set; }
    }
}