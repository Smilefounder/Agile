using Agile.Dtos;
using Agile.Dtos.API;
using Agile.Helpers;
using haha.uimoe.com.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace haha.uimoe.com.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NewList()
        {
            return View();
        }

        public ActionResult GetHotList()
        {
            return GetNewList((int)NewListTypeEnum.ByGoodCount);
        }

        public ActionResult GetNewList(int? rtype)
        {
            int page;
            int.TryParse(Request.Params["page"], out page);
            if (page < 1)
            {
                page = 1;
            }

            var url = "http://www.haha.mx/new/";
            var xpath = "/html/body/div[2]/div/div[2]/div[1]";

            var rtype1 = rtype.GetValueOrDefault(0);
            var rtype2 = (int)NewListTypeEnum.ByGoodCount;
            if (rtype1 == rtype2)
            {
                url = "http://www.haha.mx/good/day/";
                xpath = "/html/body/div[2]/div/div[2]/div[2]";
            }

            url += page;
            var response = new NewListVM
            {
                data =new List<NewListItemVM>()
            };

            try
            {
                var client = new WebClient();
                client.Encoding = Encoding.UTF8;
                var responsestr = client.DownloadString(url);

                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(responsestr);

                var list = doc.DocumentNode.SelectSingleNode(xpath);
                foreach (var item in list.ChildNodes)
                {
                    if (item.NodeType == HtmlAgilityPack.HtmlNodeType.Text)
                    {
                        continue;
                    }

                    var jokeid = item.GetAttributeValue("jid", "0");
                    var postedby = item.SelectSingleNode("div[1]/div[1]/div[1]/div[1]/a[1]");
                    var postedat = item.SelectSingleNode("div[1]/div[1]/div[1]/div[2]/span[1]");
                    var content = item.SelectSingleNode("div[1]/div[2]/p");
                    var pictureurl = item.SelectSingleNode("div[1]/div[2]/div/a[1]/img");
                    var goodcount = item.SelectSingleNode("div[1]/div[3]/div[2]/div[1]/a[1]");
                    var badcount = item.SelectSingleNode("div[1]/div[3]/div[2]/div[1]/a[2]");
                    var commentcount = item.SelectSingleNode("div[1]/div[3]/div[2]/div[2]/a[3]");

                    response.data.Add(new NewListItemVM
                    {
                        badcount = badcount == null ? 0 : StringHelper.GetNumberFromStr(badcount.InnerText),
                        commentcount = commentcount == null ? 0 : StringHelper.GetNumberFromStr(commentcount.InnerText),
                        content = content == null ? "" : content.InnerText,
                        goodcount = goodcount == null ? 0 : StringHelper.GetNumberFromStr(goodcount.InnerText),
                        jokeid = jokeid,
                        pictureurl = pictureurl == null ? null : pictureurl.GetAttributeValue("src", null),
                        postedat = postedat == null ? DateTime.Now.ToString("yyyy-MM-dd HH:mm") : StringHelper.GetDateTimeFromStr(postedat.InnerText).ToString("yyyy-MM-dd HH:mm"),
                        postedby = postedby == null ? "" : postedby.InnerText
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }

            return View("~/Views/User/GetNewList.cshtml", response);
        }
    }
}