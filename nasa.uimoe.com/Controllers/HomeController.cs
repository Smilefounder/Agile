using Agile.Attributes;
using Agile.Dtos;
using Agile.Dtos.API;
using Agile.Helpers;
using Agile.Helpers.API;
using nasa.uimoe.com.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace nasa.uimoe.com.Controllers
{
    public class HomeController : Controller
    {
        [FreeAccess]
        public ActionResult Index()
        {
            var response = default(H10033Response);

            try
            {
                var responsebase = LogicHelper.H10033(ReflectHelper.ParseFromRequest<H10033Request>());
                response = responsebase as H10033Response;
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            if (response == null)
            {
                response = new H10033Response
                {
                    data = new PagedListDto<H10033ResponseListItem>
                    {
                        RecordList = new List<H10033ResponseListItem>()
                    }
                };
            }

            var username = Session["username"] as string;
            var loged = String.IsNullOrEmpty(username) ? "0" : "1";
            ViewBag.loged = loged;

            var isPhoneRequest = StringHelper.IsPhoneRequest(Request.UserAgent);
            if (isPhoneRequest)
            {
                return View("Index_m", response);
            }

            return View(response);
        }

        [HttpGet]
        public ActionResult PostImg()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PostImg(PostImgVM vm)
        {
            try
            {
                var responsebase = LogicHelper.H10034(new H10034Request
                {
                    title = vm.imgtitle,
                    filename = vm.imgfile
                });

                if (responsebase != null && responsebase.error == 0)
                {
                    responsebase.message = "感谢您的投递，图片审核后将会显示";
                }

                return Json(responsebase);
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return Json(new { error = 1, message = "操作失败，请稍后再试" });
        }

        [HttpPost]
        public ActionResult UpdateFile()
        {
            var file1 = Request.Files["file1"];
            if (file1 == null)
            {
                return Json(new { error = 1, message = "未接收到文件" });
            }

            var extlist = new string[] { ".jpg", ".png" };
            var ext = System.IO.Path.GetExtension(file1.FileName);
            if (!extlist.Contains(ext.ToLower()))
            {
                return Json(new { error = 1, message = String.Format("不支持的文件格式：{0},请上传以下格式的文件：{1}", ext, String.Join(",", extlist)) });
            }

            var kbsize = 1.0 * file1.ContentLength / 1024;
            if (kbsize > 320)
            {
                return Json(new { error = 1, message = String.Format("最多只能上传320KB大小的图片,当前文件大小：{0}", kbsize) });
            }

            var folder = Server.MapPath("~/Uploads");
            if (!System.IO.Directory.Exists(folder))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(folder);
                }
                catch
                {
                    return Json(new { error = 1, message = "保存文件失败了" });
                }
            }

            var filename = String.Format("{0}{1}", Guid.NewGuid().ToString(), ext);
            var fullname = System.IO.Path.Combine(new string[] { folder, filename });

            file1.SaveAs(fullname);
            return Json(new { error = 0, data = filename });
        }
    }
}
