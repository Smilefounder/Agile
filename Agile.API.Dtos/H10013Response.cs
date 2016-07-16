using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.API.Dtos
{
    public class H10013Response : HBaseResponse
    {
        public List<H10013ResponseListItem> data { get; set; }

        public List<H10013ResponseListItem> webapps
        {
            get
            {
                return data.Where(w => w.apptype == (int)H10013ResponseListItemAppTypeEnum.WebPage).ToList();
            }
        }

        public List<H10013ResponseListItem> wxapps
        {
            get
            {
                return data.Where(w => w.apptype == (int)H10013ResponseListItemAppTypeEnum.Weixin).ToList();
            }
        }

        public List<H10013ResponseListItem> winapps
        {
            get
            {
                return data.Where(w => w.apptype == (int)H10013ResponseListItemAppTypeEnum.Desktop).ToList();
            }
        }

        public List<H10013ResponseListItem> iosapps
        {
            get
            {
                return data.Where(w => w.apptype == (int)H10013ResponseListItemAppTypeEnum.IOS).ToList();
            }
        }

        public List<H10013ResponseListItem> androidapps
        {
            get
            {
                return data.Where(w => w.apptype == (int)H10013ResponseListItemAppTypeEnum.Android).ToList();
            }
        }
    }

    public class H10013ResponseListItem
    {
        public int id { get; set; }

        public int apptype { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public string href { get; set; }

        public string apptypedisplay
        {
            get
            {
                switch (apptype)
                {
                    case (int)H10013ResponseListItemAppTypeEnum.Android:
                        return "Android应用";
                    case (int)H10013ResponseListItemAppTypeEnum.Desktop:
                        return "Windows应用";
                    case (int)H10013ResponseListItemAppTypeEnum.IOS:
                        return "iOS应用";
                    case (int)H10013ResponseListItemAppTypeEnum.WebPage:
                        return "网页小工具";
                    case (int)H10013ResponseListItemAppTypeEnum.Weixin:
                        return "微信公众号";
                }

                return "";
            }
        }
    }

    public enum H10013ResponseListItemAppTypeEnum
    {
        WebPage = 0,

        Weixin = 1,

        Desktop = 2,

        IOS = 3,

        Android = 4
    }
}
