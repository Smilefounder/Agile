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
            var apptype = 0;
            int.TryParse(Request.Params["apptype"], out apptype);

            var apptypedisplay = "";
            switch (apptype)
            {
                case (int)H10013ResponseListItemAppTypeEnum.Android:
                    {
                        apptypedisplay = "Android应用";
                    }
                    break;
                case (int)H10013ResponseListItemAppTypeEnum.Desktop:
                    {
                        apptypedisplay = "Windows应用";
                    }
                    break;
                case (int)H10013ResponseListItemAppTypeEnum.IOS:
                    {
                        apptypedisplay = "iOS应用";
                    }
                    break;
                case (int)H10013ResponseListItemAppTypeEnum.WebPage:
                    {
                        apptypedisplay = "网页小工具";
                    }
                    break;
                case (int)H10013ResponseListItemAppTypeEnum.Weixin:
                    {
                        apptypedisplay = "微信公众号";
                    }
                    break;
            }

            ViewBag.apptypedisplay = apptypedisplay;
            var vm = new List<H10013ResponseListItem>();

            try
            {
                var response = LogicHelper.H10065(apptype);
                if (response != null &&
                    response.Any())
                {
                    vm = response;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return View(vm);
        }

        [HttpGet]
        public ActionResult NewApp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewApp(int? apptype, string appdesc, string email, string phoneNum)
        {
            try
            {
                var response = LogicHelper.H10099(apptype, appdesc, email, phoneNum);
                return Json(response);
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return Json(new { error = 1, message = "操作失败，请稍后再试" });
        }

        [HttpGet]
        public ActionResult NewAd()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewAd(string title, string url, string imgfile, decimal? price, decimal? rate)
        {
            try
            {
                var folder1 = Server.MapPath("~/Uploads");
                var file1 = System.IO.Path.Combine(new string[] { folder1, imgfile });
                if (!System.IO.File.Exists(file1))
                {
                    return Json(new { error = 1, message = "找不到文件：" + imgfile });
                }

                //复制到广告目录下
                var folder2 = Server.MapPath("~/Content/ad");
                var file2 = System.IO.Path.Combine(new string[] { folder2, imgfile });
                System.IO.File.Copy(file1, file2, true);

                var response = LogicHelper.H10101(title, url, imgfile, price, rate);
                return Json(response);
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return Json(new { error = 1, message = "操作失败，请稍后再试" });
        }
    }
}
