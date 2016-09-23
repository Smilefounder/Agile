using Agile.Helpers;
using Agile.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace www.uimoe.com.Controllers
{
    public class AppController : Controller
    {
        public ActionResult ChooseColor()
        {
            return View();
        }

        public ActionResult FateCalc()
        {
            return View();
        }

        public ActionResult GetRandNum()
        {
            return View();
        }

        public ActionResult GetRandStr()
        {
            return View();
        }

        public ActionResult MouseClickSpeedTest()
        {
            return View();
        }

        public ActionResult RpCalc()
        {
            return View();
        }

        public ActionResult RpTest()
        {
            return View();
        }

        public ActionResult YSL()
        {
            return View();
        }

        public ActionResult CantoneseDict()
        {
            return View();
        }

        public ActionResult CantoneseDictAndroid()
        {
            return View();
        }

        public ActionResult CantoneseDictWeixin()
        {
            return View();
        }

        public ActionResult NASA()
        {
            return View();
        }

        public ActionResult Food()
        {
            return View();
        }

        public ActionResult Buy()
        {
            return View();
        }

        public ActionResult NASAAndroid()
        {
            return View();
        }

        public ActionResult GzMetroFullVer()
        {
            return View();
        }

        public ActionResult UMENote()
        {
            return View();
        }

        public ActionResult UMEMoney()
        {
            return View();
        }

        public ActionResult HahaMx()
        {
            return View();
        }

        public ActionResult UMEBrowser()
        {
            return View();
        }

        public ActionResult UMEMusic()
        {
            return View();
        }

        public ActionResult UMEVocabulary()
        {
            return View();
        }

        public ActionResult ModelToSql()
        {
            return View();
        }

        public ActionResult SqlToModel()
        {
            return View();
        }

        public ActionResult Md5()
        {
            return View();
        }

        public ActionResult Sha1()
        {
            return View();
        }

        public ActionResult Md5AndSha1()
        {
            return View();
        }

        public ActionResult QrCode()
        {
            return View();
        }

        public ActionResult UrlEncode()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ChnStopWord()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChnStopWord(string input)
        {
            var words = new List<string>();
            try
            {
                throw new Exception("分词功能暂时关闭");
            }
            catch (Exception ex)
            {
                LogHelper.WriteAsync(ex.ToString());
            }

            return Json(new { error = 0, data = words });
        }

        [HttpGet]
        public ActionResult ResolveUserAgent()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResolveUserAgent(FormCollection collect)
        {
            var ua = Request.UserAgent;
            var device = StringHelper.GetDeviceFromUserAgent(ua);
            var os = StringHelper.GetOsFromUserAgent(ua);
            var browser = Request.Browser.Browser;
            var verstr = Request.Browser.MajorVersion;
            if (verstr > 0)
            {
                browser = browser + " " + verstr;
            }

            var isphone = StringHelper.IsPhoneRequest(ua);
            return Json(new { error = 0, device = device, os = os, browser = browser, isphone = isphone });
        }

        public ActionResult NewProject()
        {
            return View();
        }

        public ActionResult IOSClient()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ClipImage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ClipImage(int? rows, int? cols, string imgfile)
        {
            var rows2 = rows.GetValueOrDefault();
            if (rows2 < 1 || rows2 > 9)
            {
                return Json(new { error = 1, message = "rows应该是1~9之间的整数" });
            }

            var cols2 = cols.GetValueOrDefault();
            if (cols2 < 1 || cols2 > 9)
            {
                return Json(new { error = 1, message = "cols应该是1~9之间的整数" });
            }

            try
            {
                var folder = Server.MapPath("~/Uploads");
                var filename = System.IO.Path.Combine(new string[] { folder, imgfile });
                ImageHelper.Clip(filename, rows2, cols2);
                return Json(new { error = 0, message = "裁剪完成" });
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
