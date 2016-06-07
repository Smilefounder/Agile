using Agile.Dtos.API;
using Agile.Helpers;
using Agile.Helpers.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace www.uimoe.com.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                var responsebase = LogicHelper.H10013(new H10013Request
                {
                    take = 10
                });

                return View(responsebase);
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
                return View(new H10013Response
                {
                    data = new List<H10013ResponseListItem>()
                });
            }
        }
    }
}
