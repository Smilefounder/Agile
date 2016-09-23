using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace cantonesedict.uimoe.com.Helpers
{
    public class CantoneseHelper
    {
        public static string GetFromWeb(string chntext)
        {
            var url = String.Format("http://m.yueyv.cn/?keyword={0}", chntext);
            var str = new WebClient().DownloadString(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(str);

            var node = doc.DocumentNode.SelectSingleNode("/html/body/div[2]/div");
            if (node == null)
            {
                return null;
            }

            var sb = "";
            foreach (var child in node.ChildNodes)
            {
                if (child.Name.ToUpper() == "AUDIO")
                {
                    var src = child.GetAttributeValue("src", "");
                    src = src.Replace("voice/", "");
                    src = src.Replace(".mp3", "");
                    sb += src + " ";
                }
            }

            return sb;
        }
    }
}