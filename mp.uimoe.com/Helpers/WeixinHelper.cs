using Agile.Helpers;
using Agile.Web.Helpers;
using mp.uimoe.com.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace mp.uimoe.com.Helpers
{
    public class WeixinHelper
    {
        /// <summary>
        /// 凭据
        /// </summary>
        private const string token = "4e4e226c17d6c2f7f63f82b27a7d7928";

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

        public static string BuildContentResponse(MP_VoiceResponse response)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("<xml>");
            sb.AppendFormat("<ToUserName><![CDATA[{0}]]></ToUserName>", response.ToUserName);
            sb.AppendFormat("<FromUserName><![CDATA[{0}]]></FromUserName>", response.FromUserName);
            sb.AppendFormat("<CreateTime><![CDATA[{0}]]></CreateTime>", DateTimeHelper.GetTimeStamp());
            sb.AppendFormat("<MsgType><![CDATA[{0}]]></MsgType>", "text");
            sb.AppendFormat("<MediaId><![CDATA[{0}]]></MediaId>", response.MediaId);
            sb.AppendFormat("</xml>");
            return sb.ToString();
        }

        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public static string GetAccessToken(string appId, string appSecret)
        {
            var url = "https://api.weixin.qq.com/cgi-bin/token?{0}";
            var client = new WebClient();

            var response = new MP_AccessTokenResponse
            {
                errcode = 40013,
                errmsg = "invalid appid"
            };

            try
            {
                var querystr = WebHelper.ToRequestStr<MP_AccessTokenRequest>(new MP_AccessTokenRequest
                {
                    appid = appId,
                    grant_type = "client_credential",
                    secret = appSecret
                });

                var responsestr = client.DownloadString(string.Format(url, querystr));
                response = SerializeHelper.ParseFromJson<MP_AccessTokenResponse>(responsestr);
            }
            catch
            { }

            if (response == null || string.IsNullOrEmpty(response.access_token))
            {
                return null;
            }

            return response.access_token;
        }

        /// <summary>
        /// 上传临时素材
        /// </summary>
        public static string UploadFile(string filepath, UploadFileTypeEnum utype, string appId, string appSecret)
        {
            var url = "https://api.weixin.qq.com/cgi-bin/material/add_material?{0}";
            var client = new WebClient();

            var response = new MP_UploadTempFileResponse
            {
                errcode = 40013,
                errmsg = "invalid appid"
            };

            try
            {
                if (!System.IO.File.Exists(filepath))
                {
                    return null;
                }

                var access_token = GetAccessToken(appId, appSecret);
                if (string.IsNullOrEmpty(access_token))
                {
                    return null;
                }

                var querystr = string.Format("access_token={0}&type={1}", access_token, Enum.GetName(typeof(UploadFileTypeEnum), utype));
                var bytes = client.UploadFile(string.Format(url, querystr), "POST", filepath);
                var responsestr = Encoding.UTF8.GetString(bytes);
                response = SerializeHelper.ParseFromJson<MP_UploadTempFileResponse>(responsestr);
            }
            catch
            { }

            if (response == null || string.IsNullOrEmpty(response.media_id))
            {
                return null;
            }

            return response.media_id;
        }

        /// <summary>
        /// 上传临时素材
        /// </summary>
        public static string UploadTempFile(string filepath, UploadFileTypeEnum utype, string appId, string appSecret)
        {
            var url = "https://api.weixin.qq.com/cgi-bin/media/upload?{0}";
            var client = new WebClient();

            var response = new MP_UploadTempFileResponse
            {
                errcode = 40013,
                errmsg = "invalid appid"
            };

            try
            {
                if (!System.IO.File.Exists(filepath))
                {
                    return null;
                }

                var access_token = GetAccessToken(appId, appSecret);
                if (string.IsNullOrEmpty(access_token))
                {
                    return null;
                }

                var querystr = string.Format("access_token={0}&type={1}", access_token, Enum.GetName(typeof(UploadFileTypeEnum), utype));
                var bytes = client.UploadFile(string.Format(url, querystr), "POST", filepath);
                var responsestr = Encoding.UTF8.GetString(bytes);
                response = SerializeHelper.ParseFromJson<MP_UploadTempFileResponse>(responsestr);
            }
            catch
            { }

            LogHelper.WriteAsync(response.errcode + ":" + response.errmsg);

            if (response == null || string.IsNullOrEmpty(response.media_id))
            {
                return null;
            }

            return response.media_id;
        }
    }

    public enum UploadFileTypeEnum
    {
        image = 0,

        voice = 1,

        video = 2,

        thumb = 3
    }
}