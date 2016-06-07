using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMEWebServer.Models;

namespace UMEWebServer.Helpers
{
    public class CoreHelper
    {
        private static List<Website> _websiteList { get; set; }

        private static List<Website> WebsiteList
        {
            get
            {
                if (_websiteList == null)
                {
                    _websiteList = new List<Website>();
                }

                return _websiteList;
            }
            set
            {
                _websiteList = value;
            }
        }

        public static List<Website> GetWebsiteList()
        {
            return WebsiteList;
        }

        public static void NewWebsite(Website model)
        {
            WebsiteList.Add(model);
        }
    }
}
