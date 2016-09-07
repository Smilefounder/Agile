using System;
using System.Linq;
using System.Web.Mvc;

namespace files.uimoe.com.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UploadFile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdateFile()
        {
            var file1 = Request.Files["file1"];
            if (file1 == null)
            {
                return Json(new { error = 1, message = "未接收到文件" });
            }

            var extlist = new string[] { ".txt",".html",".jpg", ".jpeg", ".png",".gif",".rar" };
            var ext = System.IO.Path.GetExtension(file1.FileName);
            if (!extlist.Contains(ext.ToLower()))
            {
                return Json(new { error = 1, message = string.Format("不支持的文件格式：{0},请上传以下格式的文件：{1}", ext, string.Join(",", extlist)) });
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

        public ActionResult DownloadFile()
        {
            var area = Request.Params["area"];
            if (!string.IsNullOrEmpty(area))
            {
                area = Server.UrlDecode(area);
            }

            var filename = Request.Params["filename"];
            if (!string.IsNullOrEmpty(filename))
            {
                filename = Server.UrlDecode(filename);
            }

            var filepath = Server.MapPath(string.Format("~/Content/{0}/{1}", area, filename));
            if (!System.IO.File.Exists(filepath))
            {
                return Json(new { error = 1, message = "未找到文件：" + filename }, JsonRequestBehavior.AllowGet);
            }

            var contentType = "";
            var ext = System.IO.Path.GetExtension(filepath).ToLower();
            switch (ext)
            {
                case ".txt":
                    {
                        contentType = "text/plain";
                    }
                    break;
                case ".html":
                    {
                        contentType = "text/html";
                    }
                    break;
                case ".jpg":
                    {
                        contentType = "image/jpg";
                    }
                    break;
                case ".jpeg":
                    {
                        contentType = "image/jpeg";
                    }
                    break;
                case ".png":
                    {
                        contentType = "image/png";
                    }
                    break;
                case ".gif":
                    {
                        contentType = "image/gif";
                    }
                    break;
                case ".rar":
                    {
                        contentType = "application/x-rar-compressed";
                    }
                    break;
            }

            if (string.IsNullOrEmpty(contentType))
            {
                return Json(new { error = 2, message = "未识别的文件格式：" + ext }, JsonRequestBehavior.AllowGet);
            }

            return File(filename, contentType, filename);
        }
    }
}