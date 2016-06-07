using Agile.Helpers;
using mp.uimoe.com.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace mp.uimoe.com.Helpers
{
    public class WeixinHelper
    {
        /// <summary>
        /// 凭据
        /// </summary>
        private static readonly string token = "4e4e226c17d6c2f7f63f82b27a7d7928";


        /// <summary>
        /// 验证是否是来自微信的消息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string CheckWeixinMessage(MP_AuthorizeRequest request)
        {
            var response = "不是来自微信的消息";
            if (String.IsNullOrEmpty(request.signature) ||
                String.IsNullOrEmpty(request.timestamp) ||
                String.IsNullOrEmpty(request.nonce) ||
                String.IsNullOrEmpty(request.echostr))
            {
                return response;
            }

            var tmplist = new List<string>() { token, request.timestamp, request.nonce };
            tmplist.Sort();

            string tmpstr = String.Join(String.Empty, tmplist);
            if (request.signature.ToUpper() == SecurityHelper.GetSHA1String(tmpstr).ToUpper())
            {
                return request.echostr;
            }

            return response;
        }

        public static string BuildContentResponse(MP_TextResponse response)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("<xml>");
            sb.AppendFormat("<ToUserName><![CDATA[{0}]]></ToUserName>", response.ToUserName);
            sb.AppendFormat("<FromUserName><![CDATA[{0}]]></FromUserName>", response.FromUserName);
            sb.AppendFormat("<CreateTime><![CDATA[{0}]]></CreateTime>", DateTimeHelper.GetTimeStamp());
            sb.AppendFormat("<MsgType><![CDATA[{0}]]></MsgType>", "text");
            sb.AppendFormat("<Content><![CDATA[{0}]]></Content>", response.Content);
            sb.AppendFormat("</xml>");
            return sb.ToString();
        }
    }
}