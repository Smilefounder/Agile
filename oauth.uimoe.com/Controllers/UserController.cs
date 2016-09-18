using Agile.API.Dtos;
using Agile.API.Helpers;
using Agile.Attributes;
using Agile.Helpers;
using Agile.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace oauth.uimoe.com.Controllers
{
    [FreeAccess]
    public class UserController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var response = new H10009Response
            {
                error = 1,
                message = "操作失败，请稍后再试"
            };

            try
            {
                var request = WebHelper.ParseFromRequest<H10009Request>();
                var responsebase = LogicHelper.H10009(request);
                var h10009Response = responsebase as H10009Response;
                if (h10009Response != null &&
                    h10009Response.error == 0)
                {
                    response = new H10009Response
                    {
                        error = 0,
                        message = h10009Response.message,
                        token = h10009Response.token,
                        userid = h10009Response.userid,
                        username = request.username
                    };
                }
                return Json(response);
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return Json(new { error = 1, message = "操作失败，请稍后再试" });
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(FormCollection collection)
        {
            var response = new HBaseResponse
            {
                error = 1,
                message = "操作失败，请稍后再试"
            };

            try
            {
                var h10010response = LogicHelper.H10010(WebHelper.ParseFromRequest<H10010Request>());
                if (h10010response != null)
                {
                    response = new HBaseResponse
                    {
                        error = h10010response.error,
                        message = h10010response.message
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return Json(response);
        }
    }
}