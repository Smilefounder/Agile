using Agile.Cache;
using Agile.Helpers;
using System;
using System.Collections.Generic;
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
                words = ChineseDictionary.GetWords(input);
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
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
    }
}
