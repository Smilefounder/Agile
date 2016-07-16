using Agile.API.Dtos;
using Agile.Helpers;
using Agile.API.Helpers;
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
            var vm = new H10013Response
            {
                error = 0,
                data = new List<H10013ResponseListItem>()
            };

            try
            {
                var h10013response = LogicHelper.H10013(new H10013Request
                {
                    take = 10
                });

                if (h10013response != null &&
                    h10013response.error == 0 &&
                    h10013response.data != null &&
                    h10013response.data.Any())
                {
                    vm.data = h10013response.data;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return View(vm);
        }

        public ActionResult AppList()
        {
            var id = 0;
            int.TryParse(Request.Params["id"], out id);

            var vm = new H10013ResponseListItem();

            try
            {
                var response = LogicHelper.H10065(id);
                if (response != null)
                {
                    vm = new H10013ResponseListItem
                    {
                        apptype = response.apptype,
                        description = response.description,
                        href = response.href,
                        id = response.id,
                        title = response.title
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return View(vm);
        }
    }
}
