using Agile.API.Dtos;
using Agile.API.Helpers;
using Agile.Dtos;
using Agile.Helpers;
using Agile.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace food.uimoe.com.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetIndex(int? page, int? pagesize)
        {
            var vm = new PagedListDto<H10103ResponseListItem>
            {
                RecordList = new List<H10103ResponseListItem>()
            };

            try
            {
                var request = WebHelper.ParseFromRequest<HPagedListRequest>();
                vm = LogicHelper.H10103(request.page, request.pagesize);
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return View(vm);
        }

        [HttpGet]
        public ActionResult PostImg()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PostImg(string imgfile, string postedBy, string weiboUrl, string labels)
        {
            try
            {
                var filepath = System.IO.Path.Combine(new string[] { Server.MapPath("~/Uploads"), imgfile });
                var filepath2 = System.IO.Path.Combine(new string[] { Server.MapPath("~/Content/Images"), imgfile });
                var filepath3 = System.IO.Path.Combine(new string[] { Server.MapPath("~/Content/Images/Thumb"), imgfile });
                var ht = 0;
                var wt = 0;
                System.IO.File.Copy(filepath, filepath2);
                ImageHelper.MakeThumbnail(filepath, filepath3, 480, true, out ht, out wt);

                if (LogicHelper.H10104(imgfile, postedBy, weiboUrl, labels, ht, wt))
                {
                    return Json(new { error = 0, message = "感谢您的投递，图片将在首页显示" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
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

            var extlist = new string[] { ".jpg", ".jpeg", ".png" };
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