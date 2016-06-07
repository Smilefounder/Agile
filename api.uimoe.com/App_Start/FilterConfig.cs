using api.uimoe.com.Attributes;
using System.Web;
using System.Web.Mvc;

namespace api.uimoe.com
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