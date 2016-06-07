using Agile.Helpers;
using cms.uimoe.com.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cms.uimoe.com.Helpers
{
    public class CoreHelper
    {
        public static CMS_page GetPage(string rawUrl)
        {
            var sqlstr = String.Format("select top 1 * FROM CMS_page WHERE RawUrl='{0}';",rawUrl);
            var recordlist = DataHelper.ExecuteList<CMS_page>(sqlstr);
            if (recordlist != null && recordlist.Count > 0)
            {
                return recordlist.FirstOrDefault();
            }

            return null;
        }
    }
}