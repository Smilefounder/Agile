using System.Web;
using System.Web.Mvc;
using www.llyn23.com.Attributes;

namespace www.llyn23.com
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