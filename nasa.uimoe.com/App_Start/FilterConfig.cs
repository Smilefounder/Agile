using nasa.uimoe.com.Attributes;
using System.Web;
using System.Web.Mvc;

namespace nasa.uimoe.com
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleAccessAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }
}